namespace SalonSync.Core.DTOs;

public class LicenseDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? LicenseEndDate { get; set; }
}