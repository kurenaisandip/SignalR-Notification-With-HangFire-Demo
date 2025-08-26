namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Login
{
    public int LoginID { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastLogin { get; set; }
    public bool IsActive { get; set; }
}
