using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Financial;

public interface IAccountPayableHandler
{
    Task<Response<AccountPayable?>> CreateAsync(CreateAccountPayableRequest request);
    Task<Response<AccountPayable?>> UpdateAsync(UpdateAccountPayableRequest request);
    Task<Response<AccountPayable?>> DeleteAsync(DeleteAccountPayableRequest request);
    Task<Response<AccountPayable?>> CloseAsync(CloseAccountPayableRequest request);
    Task<Response<AccountPayable?>> GetByIdAsync(GetAccountPayableByIdRequest request);
    Task<PagedResponse<List<AccountPayable>?>> GetByPeriodAsync(GetAccountPayableByPeriodRequest request);
}