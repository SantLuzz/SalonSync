using SalonSync.Core.DTOs;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Financial;

public interface ICommissionHandler
{
    Task<Response<Commission?>> CreateAsync(CreateCommissionRequest request);
    Task<Response<Commission?>> DeleteAsync(DeleteCommissionRequest request);
    Task<PagedResponse<List<UnpaidCommissionDto>?>> GetUnpaidAsync(GetUnpaidCommissionInPeriodRequest request);
}