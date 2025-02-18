using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Appointments;

public class FinishSchedulingRequest : Request
{
    [Required(ErrorMessage = "Informe o agendamento selecionado!")]
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Informe o horário Incial do agendamento!")]
    public TimeOnly StartTime { get; set; }
    
    [Required(ErrorMessage = "Informe a hora final do agendamento!")]
    public TimeOnly EndTime { get; set; }
    
    [Required(ErrorMessage = "Informe o serviço a ser executado")]
    public long ServiceId { get; set; }
    
    [Required(ErrorMessage = "Informe a data do agendamento!")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Informe o resposável pelo agendamento!")]
    public long EmployeeId { get; set; }

    public long? AppointmentGroupId { get; set; }
    
    public EAppointmentStatus Status { get; set; } = EAppointmentStatus.Completed;
    public string? CancellationReason { get; set; }
}