using System.Net.Http.Json;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Licenses;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Licenses;

public class LicenseHandler(IHttpClientFactory clientFactory) : ILicenseHandler
{
    private readonly HttpClient _client = clientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<bool> CheckAsync(string userId)
    {
        var result = await _client.GetFromJsonAsync<bool>("v1/licenses/check-license");
        return result;
    }
    
    public async Task<PagedResponse<List<LicenseDto>?>> GetAllAsync()
     =>  await _client.GetFromJsonAsync<PagedResponse<List<LicenseDto>?>>("v1/licenses/")
      ?? new PagedResponse<List<LicenseDto>?>(null, 400, "Falha ao buscar as licensas dos usuários!");

    public async Task<Response<LicenseDto?>> UpdateAsync(UpdateLicenseRequest request)
    {
        var result = await _client.PutAsJsonAsync($"v1/licenses/update", request);
        return await result.Content.ReadFromJsonAsync<Response<LicenseDto?>>()
               ?? new Response<LicenseDto?>(null, 400, "Não foi possível atualizar a licença!");
    }
}