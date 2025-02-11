using SalonSync.Core.Models.Appointments;

namespace SalonSync.Core.Models.Entities;

public class Employee
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? Position { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? Admission { get; set; }
    public decimal? Commission { get; set; }
    public string? Image { get; set; }
    public TimeOnly? StartTime { get; set; }
    public TimeOnly? EndTime { get; set; }
    public TimeOnly? LunchStart { get; set; }
    public TimeOnly? LunchEnd { get; set; }
    public string UserId { get; set; } = string.Empty;

    public List<Appointment> Appointments { get; set; } = [];
}