using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Companies;

public class CreateCompanyRequest
{
    [Required(ErrorMessage = "Informe o nome da empresa!")]
    public string Name { get; set; } = string.Empty;
    
    public string? Document { get; set; }
    
    public string? Phone { get; set; }
    
    [Required(ErrorMessage = "Informe o e-mail vinculado a empresa!")]
    public string UserId { get; set; } = string.Empty;
}