using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Services;

public class UpdateServiceRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "O nome do service deve ser informado!")]
    [MinLength(3, ErrorMessage = "O nome deve conter no minimo 3 caracteres!")]
    [MaxLength(160, ErrorMessage = "O nome deve conter no máximo 160 caracteres!")]
    public string Name { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Informe a duração do serviço em minutos!")]
    public int Duration { get; set; }
    
    [Required(ErrorMessage = "Informe o preço do serviço!")]
    public decimal Price { get; set; }
}