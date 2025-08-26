using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SignalR_Notification_With_HangFire_Demo.EF;
using SignalR_Notification_With_HangFire_Demo.EF.Entity;
using SignalR_Notification_With_HangFire_Demo.Hubs;

namespace SignalR_Notification_With_HangFire_Demo.Services;

public class AssignmentJobPayload
{
    public int TeacherLoginId { get; set; }
    public int ClassID { get; set; }
    public int SubjectID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime DueDateUtc { get; set; }
}

public class AssignmentJobRunner
{
    private readonly AppDbContext _db;
    private readonly IHubContext<NotificationHub> _hub;
    private readonly ILogger<AssignmentJobRunner> _logger;

    public AssignmentJobRunner(AppDbContext db, IHubContext<NotificationHub> hub, ILogger<AssignmentJobRunner> logger)
    {
        _db = db;
        _hub = hub;
        _logger = logger;
    }

    public async Task ProcessAssignmentAsync(AssignmentJobPayload payload)
    {
        var teacherUserId = payload.TeacherLoginId.ToString();
        try
        {
            // Validate teacher from login
            var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.LoginID == payload.TeacherLoginId && t.IsActive);
            if (teacher == null)
            {
                await _hub.Clients.User(teacherUserId).SendAsync("AssignmentProcessed", new { success = false, error = "Invalid teacher." });
                return;
            }

            // Validate class-subject mapping for this teacher
            var classSubject = await _db.ClassSubjects
                .FirstOrDefaultAsync(cs => cs.ClassID == payload.ClassID && cs.SubjectID == payload.SubjectID && cs.TeacherID == teacher.TeacherID && cs.IsActive);

            if (classSubject == null)
            {
                await _hub.Clients.User(teacherUserId).SendAsync("AssignmentProcessed", new { success = false, error = "Teacher not assigned to this Class/Subject." });
                return;
            }

            // Create assignment
            var assignment = new Assignment
            {
                ClassSubjectID = classSubject.ClassSubjectID,
                Title = payload.Title,
                Description = payload.Description ?? string.Empty,
                DueDate = payload.DueDateUtc,
                AssignedDate = DateTime.UtcNow,
                Status = "Assigned",
                IsActive = true
            };
            _db.Assignments.Add(assignment);
            await _db.SaveChangesAsync();

            // Log action taken
            var actionTaken = new ActionTaken
            {
                ActionType = "Assign Assignment",
                ActionDescription = $"Assignment {assignment.AssignmentID} assigned to Class {payload.ClassID}, Subject {payload.SubjectID}",
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _db.ActionTakens.Add(actionTaken);
            await _db.SaveChangesAsync();

            // Prepare action messages per student in class
            var students = await _db.StudentClasses
                .Include(sc => sc.Student)
                .Where(sc => sc.ClassID == payload.ClassID && sc.IsActive && sc.Student.IsActive)
                .Select(sc => sc.Student)
                .ToListAsync();

            var messages = new List<ActionMessage>(students.Count);
            foreach (var s in students)
            {
                messages.Add(new ActionMessage
                {
                    ActionTakenID = actionTaken.ActionTakenID,
                    UserID = s.StudentID,
                    Message = $"New assignment: {assignment.Title} due {assignment.DueDate:yyyy-MM-dd}",
                    SentAt = DateTime.UtcNow,
                    IsRead = false
                });
            }

            if (messages.Count > 0)
            {
                await _db.ActionMessages.AddRangeAsync(messages);
                await _db.SaveChangesAsync();
            }

            // Notify teacher and students via SignalR
            await _hub.Clients.User(teacherUserId).SendAsync("AssignmentProcessed", new
            {
                success = true,
                assignmentId = assignment.AssignmentID,
                classId = payload.ClassID,
                subjectId = payload.SubjectID
            });

            foreach (var s in students)
            {
                await _hub.Clients.User(s.LoginID.ToString()).SendAsync("NewAssignment", new
                {
                    assignmentId = assignment.AssignmentID,
                    title = assignment.Title,
                    dueDate = assignment.DueDate,
                    classId = payload.ClassID,
                    subjectId = payload.SubjectID
                });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing assignment job");
            await _hub.Clients.User(teacherUserId).SendAsync("AssignmentProcessed", new { success = false, error = "Internal error while processing the assignment." });
        }
    }
}

