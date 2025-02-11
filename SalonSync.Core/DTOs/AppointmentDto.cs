using SalonSync.Core.Enums;

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
}