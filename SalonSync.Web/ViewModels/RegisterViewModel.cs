using SalonSync.Core.Requests.Account;
using SalonSync.Core.Requests.Entities.Companies;

namespace SalonSync.Web.ViewModels;

public class RegisterViewModel
{
    public RegisterRequest RegisterRequest { get; set; } = new();
    public CreateCompanyRequest CompanyRequest { get; set; } = new();
}