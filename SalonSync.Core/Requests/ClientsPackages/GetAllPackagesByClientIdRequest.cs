namespace SalonSync.Core.Requests.ClientsPackages;

public class GetAllPackagesByClientIdRequest : PagedRequest
{
    public long ClientId { get; set; }
}