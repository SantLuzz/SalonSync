using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Employees;

public class UpdateEmployeeRequest : Request
{
    public long Id { get; set; }
    
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
    
    [MaxLength(20, ErrorMessage = "A função deve conter no máximo 50 caracteres!")]
    public string? Position { get; set; }
    
    public DateTime? Admission { get; set; }
    public decimal? Commission { get; set; }
}