namespace SalonSync.Web.ViewModels;

public class InstallmentViewModel
{
    public int Number { get; set; }
    public DateTime? DueDate { get; set; }
    public decimal Amount { get; set; }
}