namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class ActionTaken
{
    public int ActionTakenID { get; set; }
    public string ActionType { get; set; }    // e.g., 'Assign Assignment', 'Complete Assignment'
    public string ActionDescription { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }
}