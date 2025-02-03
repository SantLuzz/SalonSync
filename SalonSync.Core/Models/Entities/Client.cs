using SalonSync.Core.Enums;

namespace SalonSync.Core.Models.Entities;

public class Client
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string? PreferredService { get; set; }
    public string? Observations { get; set; }
    public EClientType Type { get; set; } = EClientType.Common;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? UpdatedAt { get; set; }
    public DateTime? LastProcedure { get; set; }
    public string? Image { get; set; }
    public string UserId { get; set; } = string.Empty;
}