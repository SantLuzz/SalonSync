using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.ClientsPackages;

public class GetActiveClientPackageRequest : Request
{
    [Required(ErrorMessage = "Cliente é obrigatório!")]
    public long ClientId { get; set; }
}