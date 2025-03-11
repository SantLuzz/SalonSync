using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.ClientsPackages;

public class InactiveClientIsActivePackageRequest : Request
{
    [Required(ErrorMessage = "Informe o cliente!")]
    public long ClientId { get; set; }
}