namespace SalonSync.Core.DTOs;

public class ClientBirthdaysDTO
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime? Birthday { get; set; }
}