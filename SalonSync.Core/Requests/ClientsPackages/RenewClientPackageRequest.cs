using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.ClientsPackages;

public class RenewClientPackageRequest : Request
{
    [Required(ErrorMessage = "Informe o cliente!")]
    public long ClientId { get; set; }

    [Required(ErrorMessage = "Informe o pacote!")]
    public long PackageId { get; set; }
    
    [Required(ErrorMessage = "Informe o incio da validade do pacote!")]
    public DateTime StartDate { get; set; }
    
    [Required(ErrorMessage = "Informe o final da validade do pacote!")]
    public DateTime EndDate { get; set; }
}