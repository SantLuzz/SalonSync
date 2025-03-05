using SalonSync.Core.Requests.Entities.Clients;

namespace SalonSync.Web.ViewModels;

public class CustomClientViewModel
{
    public UpdateClientRequest UpdateRequest { get; set; } = new();
    public UpdateImageClientRequest ImageRequest { get; set; } = new();
}