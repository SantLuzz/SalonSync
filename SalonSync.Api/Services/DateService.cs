namespace SalonSync.Api.Services;

public record DateService
{
    public DateTime StartOfWeek { get; private set; }
    public DateTime EndOfWeek { get; private set; }
    
    public void GetCurrentWeekRange()
    {
        DateTime today = DateTime.Today;
        int diff = today.DayOfWeek - DayOfWeek.Sunday;
    
        StartOfWeek = today.AddDays(-diff).Date;
        EndOfWeek = StartOfWeek.AddDays(6).Date;
    }
}