using System.Net.Http.Json;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Financial;

public class CostCenterHandler(IHttpClientFactory httpClientFactory) : ICostCenterHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<Response<CostCenter?>> CreateAsync(CreateCostCenterRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/costs-centers", request);
        return await result.Content.ReadFromJsonAsync<Response<CostCenter?>>()
            ?? new Response<CostCenter?>(null, 400, "Não foi possível criar o centro de custo!");
    }

    public async Task<Response<CostCenter?>> UpdateAsync(UpdateCostCenterRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/costs-centers/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<CostCenter?>>()
               ?? new Response<CostCenter?>(null, 400, "Não foi possível alterar o centro de custo!");
    }

    public async Task<Response<CostCenter?>> DeleteAsync(DeleteCostCenterRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/costs-centers/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<CostCenter?>>()
               ?? new Response<CostCenter?>(null, 404, "Não foi possível remover o centro de custo!");
    }

    public async Task<Response<CostCenter?>> GetByIdAsync(GetCostCenterByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<CostCenter?>>($"v1/costs-centers/{request.Id}")
           ?? new Response<CostCenter?>(null, 404, "Centro de custo não encontrado!");

    public async Task<PagedResponse<List<CostCenter>?>> GetAllAsync(GetAllCostCentersRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<CostCenter>?>>($"v1/costs-centers")
           ?? new PagedResponse<List<CostCenter>?>(null, 404, "Nenhum centro de custo encontrado!");

    public async Task<PagedResponse<List<CostCenter>?>> GetAllByTypeAsync(GetAllCostCentersByTypeRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<CostCenter>?>>($"v1/costs-centers/types?type={request.Type}")
           ?? new PagedResponse<List<CostCenter>?>(null, 404, "Nenhum centro de custo encontrado!");
}