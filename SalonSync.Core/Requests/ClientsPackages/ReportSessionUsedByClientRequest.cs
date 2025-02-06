using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.ClientsPackages;

public class ReportSessionUsedByClientRequest : Request
{
    [Required(ErrorMessage = "Informe o pacote do cliente!")]
    public long PackageId { get; set; }
    
    [Required(ErrorMessage = "Informe o cliente ao qual o pacote pertence!")]
    public long ClientId { get; set; }
    
    [Required(ErrorMessage = "Informe a quantidade de sessões utilizadas!")]
    public int SessionsUsed { get; set; }

    
    public DateTime Date { get; set; }

    public bool TemporarySession { get; set; } = false;
}