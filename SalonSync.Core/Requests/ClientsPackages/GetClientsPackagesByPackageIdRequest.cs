using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.ClientsPackages;

public class GetClientsPackagesByPackageIdRequest : PagedRequest
{
    [Required(ErrorMessage = "Informe o Id do pacote")]
    public long PackageId { get; set; }
}