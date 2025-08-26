namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class StudentClass
{
    public int StudentClassID { get; set; }
    public int StudentID { get; set; }
    public Student Student { get; set; }

    public int ClassID { get; set; }
    public Class Class { get; set; }

    public DateTime EnrollmentDate { get; set; }
    public bool IsActive { get; set; }
}