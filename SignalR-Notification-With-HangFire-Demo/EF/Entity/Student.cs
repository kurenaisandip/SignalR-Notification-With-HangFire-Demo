using SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class Student
{
    public int StudentID { get; set; }
    public int LoginID { get; set; }
    public Login Login { get; set; }  // Navigation property

    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }

    // Navigation property for related StudentClass records
    public ICollection<StudentClass> StudentClasses { get; set; }
}