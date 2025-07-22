namespace SalonSync.Core.Models.Financial;

public class AccountPayable
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string SupplierName { get; set; } = string.Empty;
    public string? SupplierDocument { get; set; }
    public decimal Amount { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime CreatedAt { get; set; }
}