using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.ClientsPackages;

public class CreateClientPackageRequest : Request
{
    [Required(ErrorMessage = "Informe a data Inicial do pacote!")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "Informe a data Final!")]
    public DateTime EndDate { get; set; }
    
    [Required(ErrorMessage = "Informe o cliente!")]
    public long ClientId { get; set; }
    
    [Required(ErrorMessage = "Informe o pacote!")]
    public long PackageId { get; set; }
}