using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GenerateScheduleWithEmployeeRequest : Request
{
    [Required(ErrorMessage = "Informe o responsável da agenda!")]
    public long EmployeeId { get; set; }

    [Required(ErrorMessage = "Informe o servilo a ser executado!")]
    [Range(0.01, long.MaxValue, ErrorMessage = "Informe o serviço para continuar!")]
    public long ServiceId { get; set; }
    public long ClientId { get; set; }
    
    [Required(ErrorMessage = "Informe a data de agendamento!")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Informe a hora inicial do agendamento!")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "Informe a hora final do agendamento!")]
    public TimeOnly EndTime { get; set; }

    public bool ClientUsedPackage { get; set; } = false;
}