using SalonSync.Core.Enums;

namespace SalonSync.Web.ViewModels;

public class PaymentDetailsViewModel
{
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime? PaidOrReceivedAt { get; set; }
    public long EntityId { get; set; }
    public EEntityType EntityType { get; set; } = EEntityType.DirectTransaction;
    public ECostCenterType CenterType { get; set; } = ECostCenterType.Income;
    public long CostCenterId { get; set; }
}