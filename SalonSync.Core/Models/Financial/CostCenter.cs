using SalonSync.Core.Enums;

namespace SalonSync.Core.Models.Financial;

public class CostCenter
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public ECostCenterType Type { get; set; } = ECostCenterType.Income;
}