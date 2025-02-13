namespace SalonSync.Core.Models.Entities;

public class Company
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Document { get; set; }
    public string? Phone { get; set; }
    public string UserId { get; set; } = string.Empty;
    public Guid Code { get; set; } = Guid.NewGuid();
}