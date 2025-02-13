using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GetAppointmentByEmployeeRequest : Request
{
    [Required(ErrorMessage = "Informe o profissional responsável!")]
    public long EmployeeId { get; set; }
    
    public DateTime Date { get; set; } = DateTime.Today;
}