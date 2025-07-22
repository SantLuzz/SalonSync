using SalonSync.Core.Enums;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Models.Services;

namespace SalonSync.Core.DTOs;

public class AppointmentDto
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
    public EntityDto? Client { get; set; }
    public EntityDto? Employee { get; set; }
    public Service? Service { get; set; }
    public ClientPackageAppointment? ClientPackageAppointments { get; set; } = new();
}