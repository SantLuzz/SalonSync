namespace SalonSync.Core.Requests.Appointments;

public class GenerateAutoScheduleRequest : Request
{
    public List<GenerateScheduleWithEmployeeRequest> Schedules { get; set; } = [];
}