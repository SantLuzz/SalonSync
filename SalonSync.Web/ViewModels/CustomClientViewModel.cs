using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Clients;

namespace SalonSync.Web.ViewModels;

public class CustomClientViewModel
{
    public UpdateClientRequest UpdateRequest { get; set; } = new();
    public UpdateImageClientRequest ImageRequest { get; set; } = new();
    public ClientPackage? ClientPackage { get; set; }
    public CreateClientPackageRequest CreateClientPackageRequest { get; set; } = new();
}