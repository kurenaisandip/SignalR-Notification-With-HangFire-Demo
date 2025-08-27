using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SignalR_Notification_With_HangFire_Demo.Services;
using System.ComponentModel.DataAnnotations;

namespace SignalR_Notification_With_HangFire_Demo.Controller;

[ApiController]
[Route("api/teacher")]
[Authorize(Roles = "Teacher")]
public class TeacherController: ControllerBase
{
    private readonly AssignmentService _assignmentService;

    public TeacherController(AssignmentService assignmentService)
    {
        _assignmentService = assignmentService;
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

        var teacherLoginIdStr = User.FindFirst("loginId")?.Value;
        if (!int.TryParse(teacherLoginIdStr, out var teacherLoginId))
        {
            return Unauthorized("Missing or invalid loginId claim.");
        }

        await _assignmentService.QueueAssignmentAsync(
            teacherLoginId,
            request.ClassID,
            request.SubjectID,
            request.Title,
            request.Description,
            request.DueDate.ToUniversalTime(),
            HttpContext.RequestAborted);

        return Accepted(new { message = "Assignment queued for processing." });
    }
}