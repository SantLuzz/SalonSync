namespace SalonSync.Core.Requests.Licenses;

public class UpdateLicenseRequest : Request
{
    public DateTime LicenseEndDate { get; set; }
}