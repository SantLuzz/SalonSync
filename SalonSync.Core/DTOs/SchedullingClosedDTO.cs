using SalonSync.Core.Enums;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Core.DTOs;

public class SchedullingClosedDTO
{
    public string UserId { get; set; } = String.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int ServiceDuration { get; set; }
    public decimal ServiceValue { get; set; }
    public EAppointmentStatus Status { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = [];
}