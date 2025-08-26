using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace SignalR_Notification_With_HangFire_Demo.Hubs;

public class CustomUserIdProvider : IUserIdProvider
{
    public string? GetUserId(HubConnectionContext connection)
    {
        // Prefer our custom 'loginId' claim; fallback to NameIdentifier
        return connection.User?.FindFirst("loginId")?.Value
            ?? connection.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }
}

