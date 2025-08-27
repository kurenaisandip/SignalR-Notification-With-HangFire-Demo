using Hangfire;
using Microsoft.AspNetCore.SignalR;
using SignalR_Notification_With_HangFire_Demo.Hubs;

namespace SignalR_Notification_With_HangFire_Demo.Services;

public class AssignmentService
{
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IHubContext<NotificationHub> _hub;

    public AssignmentService(IBackgroundJobClient backgroundJobs, IHubContext<NotificationHub> hub)
    {
        _backgroundJobs = backgroundJobs;
        _hub = hub;
    }

    public async Task QueueAssignmentAsync(
        int teacherLoginId,
        int classId,
        int subjectId,
        string title,
        string? description,
        DateTime dueDateUtc,
        CancellationToken ct = default)
    {
        var payload = new AssignmentJobPayload
        {
            TeacherLoginId = teacherLoginId,
            ClassID = classId,
            SubjectID = subjectId,
            Title = title,
            Description = description,
            DueDateUtc = dueDateUtc
        };

        // Enqueue background job for processing
        _backgroundJobs.Enqueue<AssignmentJobRunner>(runner => runner.ProcessAssignmentAsync(payload));

        // Notify teacher that the assignment was queued
        await _hub.Clients.User(teacherLoginId.ToString()).SendAsync("AssignmentQueued", new
        {
            message = "Assignment queued for processing.",
            title,
            dueDate = dueDateUtc
        }, ct);
    }
}

