using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Appointments;

public class GetAvailableDateByCompanyRequest
{
    [Required(ErrorMessage = "Informe o código do profissional!")]
    public Guid Code { get; set; }
}