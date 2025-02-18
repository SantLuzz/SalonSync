using SalonSync.Core.Enums;

namespace SalonSync.Core.Models.Appointments;

public class AppointmentCancellationHistory
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long AppointmentId { get; set; }
    public DateTime Date { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public EAppointmentStatus Status { get; set; } = EAppointmentStatus.Free;
    public long? ClientId { get; set; }
    public long EmployeeId { get; set; }
    public long? ServiceId { get; set; }
    public long? AppointmentGroupId { get; set; }
    public string? CancellationReason { get; set; }
    public string CanceledBy { get; set; } = String.Empty;
    public DateTime CanceledAt { get; set; }
}