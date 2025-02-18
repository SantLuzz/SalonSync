using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Entities.Clients;

public class CreateClientRequest : Request
{
    [Required(ErrorMessage = "O nome é obrigatório!")]
    [MinLength(3, ErrorMessage = "O nome deve conter ao menos 3 caracteres!")]
    [MaxLength(255, ErrorMessage = "O nome deve conter no máximo 255 caracteres!")]
    public string Name { get; set; } = string.Empty;
    
    [EmailAddress(ErrorMessage = "Informe um e-mail válido!")]
    [Required(ErrorMessage = "O e-mail é obrigatório!")]
    [MaxLength(160, ErrorMessage = "O endereço de e-mail deve conter no máximo 160 caracteres!")]
    public string Email { get; set; } = string.Empty;
    
    [MaxLength(20, ErrorMessage = "O telefone deve conter no máximo 20 caracteres!")]
    public string? Phone { get; set; }
    
    [MaxLength(255, ErrorMessage = "As preferências do cliente deve conter no máximo 255 caracteres!")]
    public string? PreferredService { get; set; }
    
    [MaxLength(255, ErrorMessage = "As observações deve conter no máximo 255 caracteres!")]
    public string? Observations { get; set; }
    
    [Required(ErrorMessage = "O tipo do cliente é obrigatório!")]
    public EClientType Type { get; set; } = EClientType.Common;
    
    public DateTime? Birthdate { get; set; }
    
}