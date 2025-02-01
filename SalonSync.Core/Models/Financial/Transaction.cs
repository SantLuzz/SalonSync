namespace SalonSync.Core.Models.Financial;

public class Transaction
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public DateTime? PaidOrReceivedAt { get; set; }
    public decimal Amount { get; set; }
    public long CostCenterId { get; set; }
    public CostCenter CostCenter { get; set; } = null!;

}