using SalonSync.Web.Common;

namespace SalonSync.Web.ViewModels;

public class EmployeeViewModel
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public string PhoneFormatted => Masks.ApplyPhoneMask(Phone ?? string.Empty);    
    public string? Position { get; set; }
    public DateTime? Admission { get; set; }
    public string AdmissionFormatted  => Admission.HasValue ? Admission.Value.ToString("dd/MM/yyyy") : string.Empty;
    public decimal? Commission { get; set; }
}