using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Clients;

public class UpdateImageClientRequest : Request
{
    public long Id { get; set; }
    public string? Base64Image { get; set; } = string.Empty;
    public string? UrlImage { get; set; }
}