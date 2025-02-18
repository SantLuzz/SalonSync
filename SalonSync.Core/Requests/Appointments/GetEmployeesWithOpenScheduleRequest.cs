using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GetEmployeesWithOpenScheduleRequest : Request
{
    [Required(ErrorMessage = "Informe a data requisitada para o agendamento!")]
    public DateTime Date { get; set; }
}