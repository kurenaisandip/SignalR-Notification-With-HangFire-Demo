using SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Assignment
{
    public int AssignmentID { get; set; }
    public int ClassSubjectID { get; set; }
    public ClassSubject ClassSubject { get; set; }

    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime AssignedDate { get; set; }
    public string Status { get; set; }
    public bool IsActive { get; set; }
}