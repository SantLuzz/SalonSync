using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Clients;

public class UpdateImageClientRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Informe a imagem do cliente!")]
    public string Base64Image { get; set; } = string.Empty;
}