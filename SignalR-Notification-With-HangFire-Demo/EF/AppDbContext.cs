using Microsoft.EntityFrameworkCore;

namespace SignalR_Notification_With_HangFire_Demo.EF;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}