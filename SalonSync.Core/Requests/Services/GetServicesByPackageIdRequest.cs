namespace SalonSync.Core.Requests.Services;

public class GetServicesByPackageIdRequest : PagedRequest
{
    public long PackageId { get; set; }
}