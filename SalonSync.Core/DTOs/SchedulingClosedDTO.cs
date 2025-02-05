namespace SalonSync.Core.DTOs;

public class SchedulingClosedDTO
{
    public List<long> Ids { get; set; }
    public string UserId { get; set; } = String.Empty;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int ServiceDuration { get; set; }
    public decimal ServiceValue { get; set; }
}