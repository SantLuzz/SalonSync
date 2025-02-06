using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.PackagesServices;

public class CreatePackageServiceRequest : Request
{
    [Required(ErrorMessage = "O pacote é obrigatório!")]
    public long PackageId { get; set; }

    [Required(ErrorMessage = "Informe os serviços vinculados ao pacote!")]
    public IEnumerable<ServicesInPackage> Services { get; set; } = [];
}

public class ServicesInPackage
{
    [Required(ErrorMessage = "O Serviço é obrigatório!")]
    public long ServiceId { get; set; }
    [Required(ErrorMessage = "Informe a quantidade de sessões!")]
    public int SessionsPerService { get; set; }
}