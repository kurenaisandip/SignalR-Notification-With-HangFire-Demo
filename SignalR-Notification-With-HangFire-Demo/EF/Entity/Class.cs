using SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Class
{
    public int ClassID { get; set; }
    public string ClassName { get; set; }
    public string ClassCode { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    // Navigation property for related TeacherClass records
    public ICollection<TeacherClass> TeacherClasses { get; set; }

    // Navigation property for related StudentClass records
    public ICollection<StudentClass> StudentClasses { get; set; }

    // Navigation property for related ClassSubject records
    public ICollection<ClassSubject> ClassSubjects { get; set; }
}