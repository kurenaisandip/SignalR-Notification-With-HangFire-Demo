using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using SignalR_Notification_With_HangFire_Demo.Hubs;
using SignalR_Notification_With_HangFire_Demo.Services;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace SignalR_Notification_With_HangFire_Demo.Controller;

[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
public class TeacherController: ControllerBase
{
    private readonly IBackgroundJobClient _backgroundJobs;
    private readonly IHubContext<NotificationHub> _hub;

    public TeacherController(IBackgroundJobClient backgroundJobs, IHubContext<NotificationHub> hub)
    {
        _backgroundJobs = backgroundJobs;
        _hub = hub;
    }

    public class PostAssignmentRequest
    {
        [Required]
        public int ClassID { get; set; }
        [Required]
        public int SubjectID { get; set; }
        [Required]
        public string Title { get; set; } = default!;
        public string? Description { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
    }

    [HttpPost("assignments")] 
    public async Task<IActionResult> PostAssignment([FromBody] PostAssignmentRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var teacherLoginId = User.FindFirst("loginId")?.Value;
        if (string.IsNullOrWhiteSpace(teacherLoginId))
        {
            return Unauthorized("Missing loginId claim.");
        }

        var payload = new AssignmentJobPayload
        {
            TeacherLoginId = int.Parse(teacherLoginId),
            ClassID = request.ClassID,
            SubjectID = request.SubjectID,
            Title = request.Title,
            Description = request.Description,
            DueDateUtc = request.DueDate.ToUniversalTime()
        };

        // Enqueue background job
        _backgroundJobs.Enqueue<AssignmentJobRunner>(runner => runner.ProcessAssignmentAsync(payload));

        // Notify teacher immediately that the job is queued
        await _hub.Clients.User(teacherLoginId).SendAsync("AssignmentQueued", new {
            message = "Assignment queued for processing.",
            title = request.Title,
            dueDate = request.DueDate
        });

        return Accepted(new { message = "Assignment queued for processing." });
    }
}