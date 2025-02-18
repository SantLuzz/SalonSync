using SalonSync.Core.Models.Appointments;

namespace SalonSync.Core.DTOs;

public class AppointmentCreatedDto
{
    public string UserId { get; set; } = string.Empty;
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public long ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public long ServiceId { get; set; }
    public string ServiceDescription { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = [];
}