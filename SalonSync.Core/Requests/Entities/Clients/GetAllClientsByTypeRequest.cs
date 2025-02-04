using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Entities.Clients;

public class GetAllClientsByTypeRequest : PagedRequest
{
    public EClientType Type { get; set; }
}