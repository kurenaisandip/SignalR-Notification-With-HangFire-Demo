namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class ClassSubject
{
    public int ClassSubjectID { get; set; }
    public int ClassID { get; set; }
    public Class Class { get; set; }

    public int SubjectID { get; set; }
    public Subject Subject { get; set; }

    public int TeacherID { get; set; }
    public Teacher Teacher { get; set; }

    public bool IsActive { get; set; }
}