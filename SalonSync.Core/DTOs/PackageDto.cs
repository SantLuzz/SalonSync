namespace SalonSync.Core.DTOs;

public class PackageDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public List<PackageServiceDto> PackageServiceDtos { get; set; } = [];
}
