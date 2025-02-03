using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Clients;

public class UpdateLastedProcedureClientRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Informe a data do ultimo procedimento do cliente!")]
    public DateTime? LastProcedure { get; set; }
}