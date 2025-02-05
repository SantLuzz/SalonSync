using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GetAvailableHoursByServicesAndEmployeeRequest : Request
{
    [Required(ErrorMessage = "Informe o responsável da agenda!")]
    public long EmployeeId { get; set; }

    [Required(ErrorMessage = "Informe o serviço a ser executado!")]
    public long ServiceId { get; set; }

    public DayOfWeek DayOfWeek { get; set; }
}