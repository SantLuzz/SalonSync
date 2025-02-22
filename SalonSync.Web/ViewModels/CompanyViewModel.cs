using SalonSync.Core.Requests.Entities.Companies;

namespace SalonSync.Web.ViewModels;

public class CompanyViewModel
{
    public UpdateCompanyRequest UpdateRequest { get; set; } = new();
    public GenerateCodeByCompanyRequest CodeRequest { get; set; } = new();
    public string? Url { get; set; }
}