using SalonSync.Core.Models.Appointments;

namespace SalonSync.Core.Models.Financial;

public class Commission
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }

    public Appointment? Appointment { get; set; }
}