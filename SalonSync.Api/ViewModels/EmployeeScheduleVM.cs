namespace SalonSync.Api.ViewModels;

public class EmployeeScheduleVM
{
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public TimeOnly LunchStart { get; set; }
    public TimeOnly LunchEnd { get; set; }
}