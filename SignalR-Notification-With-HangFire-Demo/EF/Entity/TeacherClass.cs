namespace SignalR_Notification_With_HangFire_Demo.EF.Entity;

public class TeacherClass
{
    public int TeacherClassID { get; set; }
    public int TeacherID { get; set; }
    public Teacher Teacher { get; set; }  // Navigation property

    public int ClassID { get; set; }
    public Class Class { get; set; }  // Navigation property

    public bool IsActive { get; set; }
}
