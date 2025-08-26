using Microsoft.AspNetCore.Mvc;

namespace SignalR_Notification_With_HangFire_Demo.Controller;

[ApiController]
public class StudentController : ControllerBase
{
    // GET
    public Task Index()
    {
        return Task.CompletedTask;
    }
}