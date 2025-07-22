using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class OpenAppointmentByEmployeeRequest : Request
{
    [Required(ErrorMessage = "Informe o responsável da agenda!")]
    public long EmployeeId { get; set; }

    [Required(ErrorMessage = "Informe a data Inicial da agenda!")]
    public DateTime? StartDate { get; set; } = null!;
    
    [Required(ErrorMessage = "Informe a data Final da agenda!")]
    public DateTime? EndDate { get; set; }= null!;
}