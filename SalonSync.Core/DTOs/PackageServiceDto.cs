namespace SalonSync.Core.DTOs;

public class PackageServiceDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public long PackageId { get; set; }
    public long ServiceId { get; set; }
    public string ServiceName { get; set; } = string.Empty;
    public int Duration { get; set; }
    public decimal Price { get; set; }
    public int SessionsPerService { get; set; }
}