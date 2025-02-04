using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Entities;

public interface IClientHandler
{
    Task<Response<Client?>> CreateAsync(CreateClientRequest request);
    Task<Response<Client?>> UpdateAsync(UpdateClientRequest request);
    Task<Response<Client?>> UpdateImageAsync(UpdateImageClientRequest request);
    Task<Response<Client?>> UpdateLastedProcedureAsync(UpdateLastedProcedureClientRequest request);
    Task<Response<Client?>> DeleteAsync(DeleteClientRequest request);
    Task<Response<Client?>> GetByIdAsync(GetClientByIdRequest request);
    Task<PagedResponse<List<Client>?>> GetAllAsync(GetAllClientsRequest request);
    Task<PagedResponse<List<Client>?>> GetAllByTypeAsync(GetAllClientsByTypeRequest request);
}