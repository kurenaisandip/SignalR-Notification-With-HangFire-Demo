namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class ActionMessage
{
    public int ActionMessageID { get; set; }
    public int ActionTakenID { get; set; }
    public ActionTaken ActionTaken { get; set; }

    public int UserID { get; set; }   // Can be TeacherID or StudentID
    public string Message { get; set; }
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
}