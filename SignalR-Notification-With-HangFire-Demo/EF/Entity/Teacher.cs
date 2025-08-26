namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Teacher
{
    public int TeacherID { get; set; }
    public int LoginID { get; set; }
    public Login Login { get; set; }  // Navigation property

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime HireDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation property for related TeacherClass records
    public ICollection<TeacherClass> TeacherClasses { get; set; }
    public ICollection<ClassSubject> ClassSubjects { get; set; }
}
