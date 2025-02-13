using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Employees;

public class GenerateUniqueEmployeeCodeRequest : Request
{
    [Required(ErrorMessage = "Informe o profissional!")]
    public long EmployeeId { get; set; }
}