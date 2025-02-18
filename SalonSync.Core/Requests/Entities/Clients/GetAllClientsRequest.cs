namespace SalonSync.Core.Requests.Entities.Clients;

public class GetAllClientsRequest : PagedRequest
{
    public long ClientId { get; set; }
}