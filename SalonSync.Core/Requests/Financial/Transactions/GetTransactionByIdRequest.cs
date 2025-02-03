namespace SalonSync.Core.Requests.Financial.Transactions;

public class GetTransactionByIdRequest : Request
{
    public long Id { get; set; }
}