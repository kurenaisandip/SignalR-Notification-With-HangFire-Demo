using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace SignalR_Notification_With_HangFire_Demo.Hubs;

[Authorize]
public class NotificationHub : Hub
{
    // Optional: server methods if you want to call from clients
}

