namespace SalonSync.Core.Requests.Appointments;

public class GetAvailableDateRequest : Request
{
    public long? EmployeeId { get; set; }
}