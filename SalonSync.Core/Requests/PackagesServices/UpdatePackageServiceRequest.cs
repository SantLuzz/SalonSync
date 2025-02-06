using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.PackagesServices;

public class UpdatePackageServiceRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Informe a quantidade de sessões para o serviço!")]
    public int SessionsPerService { get; set; }
}