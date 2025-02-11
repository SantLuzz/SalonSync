using SalonSync.Core.Enums;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Models.Services;

namespace SalonSync.Core.Models.Appointments;

public class Appointment
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public EAppointmentStatus Status { get; set; } = EAppointmentStatus.Free;
    public long? ClientId { get; set; }
    public long EmployeeId { get; set; }
    public long? ServiceId { get; set; }
    public long? AppointmentGroupId { get; set; }
    public Client? Client { get; set; }
    public Employee? Employee { get; set; }
    public Service? Service { get; set; }
    public AppointmentGroup? AppointmentGroup { get; set; }
}