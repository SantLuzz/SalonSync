namespace SalonSync.Core.Requests.Financial.Commissions;

public class GetUnpaidCommissionInPeriodRequest : Request
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}