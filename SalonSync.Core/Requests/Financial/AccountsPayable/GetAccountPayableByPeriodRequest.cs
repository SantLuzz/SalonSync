using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Financial.AccountsPayable;

public class GetAccountPayableByPeriodRequest : PagedRequest
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public EAccountStatus AccountStatus { get; set; } = EAccountStatus.Open;
}