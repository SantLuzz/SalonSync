using System.Text.Json.Serialization;

namespace SalonSync.Core.Models.Entities;

public class Company
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid Code { get; set; } = Guid.NewGuid();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    [JsonIgnore]
    public string LicenseEndDate { get; set; } = string.Empty;
}