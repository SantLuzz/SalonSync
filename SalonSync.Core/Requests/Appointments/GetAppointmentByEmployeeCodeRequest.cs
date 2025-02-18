using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GetAppointmentByEmployeeCodeRequest 
{
    [Required(ErrorMessage = "Informe o código do profissional!")]
    public Guid Code { get; set; }
    [Required(ErrorMessage = "Informe a data desejada!")]
    public DateTime Date { get; set; }
}