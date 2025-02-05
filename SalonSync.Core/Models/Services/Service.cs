namespace SalonSync.Core.Models.Services;

public class Service
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Duration { get; set; }
    public decimal Price { get; set; }
}