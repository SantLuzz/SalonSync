using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Companies;

public class UpdateCompanyRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "Informe o nome da empresa!")]
    public string Name { get; set; } = string.Empty;
    
    public string? Document { get; set; }
    
    public string? Phone { get; set; }
}