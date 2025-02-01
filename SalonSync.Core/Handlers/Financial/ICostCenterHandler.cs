using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Financial;

public interface ICostCenterHandler
{
    Task<Response<CostCenter?>> CreateAsync(CreateCostCenterRequest request);
    Task<Response<CostCenter?>> UpdateAsync(UpdateCostCenterRequest request);
    Task<Response<CostCenter?>> DeleteAsync(DeleteCostCenterRequest request);
    Task<Response<CostCenter?>> GetByIdAsync(GetCostCenterByIdRequest request);
    Task<PagedResponse<List<CostCenter>?>> GetAllAsync(GetAllCostCentersRequest request);
    Task<PagedResponse<List<CostCenter>?>> GetAllByTypeAsync (GetAllCostCentersByTypeRequest request);
}