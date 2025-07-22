using System.Net.Http.Json;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Services;

public class ServiceHandler(IHttpClientFactory httpClientFactory) : IServiceHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<Response<Service?>> CreateAsync(CreateServiceRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/services", request);
        return await result.Content.ReadFromJsonAsync<Response<Service?>>()
            ?? new Response<Service?>(null, 400, "Não foi possível cadastrar o serviço!");
    }

    public async Task<Response<Service?>> UpdateAsync(UpdateServiceRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/services/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Service?>>()
               ?? new Response<Service?>(null, 404, "Serviço não encontrado!");
    }

    public async Task<Response<Service?>> DeleteAsync(DeleteServiceRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/services/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Service?>>()
               ?? new Response<Service?>(null, 404, "Serviço não encontrado!");
    }

    public async Task<Response<Service?>> GetByIdAsync(GetServiceByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<Service?>>($"v1/services/{request.Id}")
           ?? new Response<Service?>(null, 404, "Serviço não encontrado!");

    public async Task<PagedResponse<List<Service>?>> GetByPackageIdAsync(GetServicesByPackageIdRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Service>?>>(
               $"v1/services/package/{request.PackageId}")
           ?? new PagedResponse<List<Service>?>(null, 404, "Serviços não encontrados!");

    public async Task<PagedResponse<List<Service>?>> GetAllAsync(GetAllServicesRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Service>?>>($"/v1/services")
           ?? new PagedResponse<List<Service>?>(null, 404, "Serviços não encontrado!");
}