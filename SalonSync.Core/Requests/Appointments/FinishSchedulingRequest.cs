using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class FinishSchedulingRequest : Request
{
    [Required(ErrorMessage = "Informe o horário Incial do agendamento!")]
    public TimeOnly StartDate { get; set; }
    
    [Required(ErrorMessage = "Informe o serviço a ser executado")]
    public long ServiceId { get; set; }
    
    [Required(ErrorMessage = "Informe a data do agendamento!")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Informe o resposável pelo agendamento!")]
    public long EmployeeId { get; set; }
}