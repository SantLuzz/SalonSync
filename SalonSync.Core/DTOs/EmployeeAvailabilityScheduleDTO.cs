namespace SalonSync.Core.DTOs;

public class EmployeeAvailabilityScheduleDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<DateTime> AvailableDays { get; set; } = [];
    public int CountDays { get; set; }
    public string UserId { get; set; } = String.Empty;
}