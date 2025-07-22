namespace SalonSync.Core.DTOs;

public class EmployeeAvailabilitySlotsDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Dictionary<TimeOnly, TimeOnly> AvailableSlots  { get; set; } = [];
    public DateTime Date { get; set; }
    public DateTime? NextAvaliabeDate { get; set; }
    public string UserId { get; set; } = String.Empty;
    public List<Hours> Hours { get; set; } = [];

    public void GetHours()
    {
        if(!AvailableSlots.Any()) return;

        Hours = AvailableSlots.Select(x => new Hours(x.Key, x.Value))
            .ToList();
    }
}

public record Hours(TimeOnly StartTime, TimeOnly EndTime)
{
    
}