using System.Net.Http.Json;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Entities;

public class ClientHandler(IHttpClientFactory httpClientFactory) : IClientHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    
    public async Task<Response<Client?>> CreateAsync(CreateClientRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/clients", request);
        return await result.Content.ReadFromJsonAsync<Response<Client?>>()
            ?? new Response<Client?>(null, 400, "Não foi possível cadastrar o cliente!");
    }

    public async Task<Response<Client?>> UpdateAsync(UpdateClientRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/clients/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Client?>>()
               ?? new Response<Client?>(null, 404, "Não foi possível alterar o cliente!");
    }

    public async Task<Response<Client?>> UpdateImageAsync(UpdateImageClientRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"/v1/clients/{request.Id}/profile-image", request);
        return await result.Content.ReadFromJsonAsync<Response<Client?>>()
               ?? new Response<Client?>(null, 404, "Não foi possível alterar a imagem do cliente!");
    }

    public async Task<Response<Client?>> UpdateLastedProcedureAsync(UpdateLastedProcedureClientRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/clients/lasted-procedures/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Client?>>()
               ?? new Response<Client?>(null, 404, "Não foi possível informar a data do ultimo procedimento do cliente!");
    }

    public async Task<Response<Client?>> DeleteAsync(DeleteClientRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/clients/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Client?>>()
               ?? new Response<Client?>(null, 404, "Não foi possível deletar o cliente!");
    }

    public async Task<Response<Client?>> GetByIdAsync(GetClientByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<Client?>>($"v1/clients/{request.Id}")
        ?? new Response<Client?>(null, 404, "Cliente não encontrado!");

    public async Task<PagedResponse<List<ClientBirthdaysDTO>?>> GetBirthdaysOfWeek(GetBirthdaysOfWeekRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<ClientBirthdaysDTO>?>>("v1/clients/birthdays")
           ?? new PagedResponse<List<ClientBirthdaysDTO>?>(null, 404, "Nenhum cliente encontrado!");

    public async Task<PagedResponse<List<Client>?>> GetAllAsync(GetAllClientsRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Client>?>>("v1/clients")
           ?? new PagedResponse<List<Client>?>(null, 404, "Nenhum cliente encontrado!");

    public async Task<PagedResponse<List<Client>?>> GetAllByTypeAsync(GetAllClientsByTypeRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Client>?>>($"/v1/clients/types?type={(int)request.Type}")
           ?? new PagedResponse<List<Client>?>(null, 404, "Nenhum cliente encontrado!");
}