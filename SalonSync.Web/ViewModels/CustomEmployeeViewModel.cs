using SalonSync.Core.Requests.Entities.Employees;

namespace SalonSync.Web.ViewModels;

public class CustomEmployeeViewModel
{
    public UpdateEmployeeRequest UpdateRequest { get; set; } = new();
    public UpdateImageEmployeeRequest ImageRequest { get; set; } = new();
    public GenerateUniqueEmployeeCodeRequest CodeRequest { get; set; } = new();
    public string? Url { get; set; }
}