using System.Net.Http.Json;
using System.Text;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Entities;

public class CompanyHandler(IHttpClientFactory httpClientFactory) : ICompanyHandler
{
    private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    
    public async Task<Response<Company?>> CreateAsync(CreateCompanyRequest request)
    {
        var result = await _client.PostAsJsonAsync("v1/companies", request);
        return await result.Content.ReadFromJsonAsync<Response<Company?>>()
            ?? new Response<Company?>(null, 400, "Falha ao criar a empresa!");
    }

    public async Task<Response<Company?>> UpdateAsync(UpdateCompanyRequest request)
    {
        var result = await _client.PutAsJsonAsync("v1/companies", request);
        return await result.Content.ReadFromJsonAsync<Response<Company?>>()
            ?? new Response<Company?>(null, 400, "Falha ao atualizar a empresa!");
    }

    public async Task<Response<Company?>> DeleteAsync(DeleteCompanyRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<Company?>> GetAsync(GetCompanyRequest request)
        => await _client.GetFromJsonAsync<Response<Company?>>("v1/companies")
                ?? new Response<Company?>(null, 404, "Empresa não encontrada!");

    public async Task<Response<Company?>> GenerateCodeAsync(GenerateCodeByCompanyRequest request)
    {
        var emptyContent = new StringContent("{}", Encoding.UTF8,  "application/json");
        var result = await _client.PutAsJsonAsync("v1/companies/generate-code", emptyContent);
        return await result.Content.ReadFromJsonAsync<Response<Company?>>()
            ?? new Response<Company?>(null, 400, "Falha ao gerar o código da empresa!");
    }

    public async Task<Company?> GetByCodeAsync(Guid code)
        => await _client.GetFromJsonAsync<Company?>("v1/companies")
           ?? null;
}