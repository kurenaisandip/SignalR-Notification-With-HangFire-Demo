namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Subject
{
    public int SubjectID { get; set; }
    public string SubjectName { get; set; }

    // Navigation property for related ClassSubject records
    public ICollection<ClassSubject> ClassSubjects { get; set; }
}
