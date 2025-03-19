namespace SalonSync.Core.Requests.Financial.AccountsPayable;

public class CloseAccountPayableRequest : Request
{
    public long Id { get; set; }
    public DateTime PaymentDate { get; set; }
}