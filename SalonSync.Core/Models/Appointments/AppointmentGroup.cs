namespace SalonSync.Core.Models.Appointments;

public class AppointmentGroup
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public int TotalDuration { get; set; }
}