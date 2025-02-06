using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Services;

public interface IServiceHandler
{
    Task<Response<Service?>> CreateAsync(CreateServiceRequest request);
    Task<Response<Service?>> UpdateAsync(UpdateServiceRequest request);
    Task<Response<Service?>> DeleteAsync(DeleteServiceRequest request);
    Task<Response<Service?>> GetByIdAsync(GetServiceByIdRequest request);
    Task<PagedResponse<List<Service>?>> GetAllAsync(GetAllServicesRequest request);
}