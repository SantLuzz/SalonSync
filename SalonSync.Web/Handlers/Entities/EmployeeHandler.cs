using System.Net.Http.Json;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Entities;

public class EmployeeHandler(IHttpClientFactory httpClientFactory) : IEmployeeHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<Response<Employee?>> CreateAsync(CreateEmployeeRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/employees", request);
        return await result.Content.ReadFromJsonAsync<Response<Employee?>>()
            ?? new Response<Employee?>(null, 400, "Não foi possível cadastrar o funcionário!");
    }

    public async Task<Response<Employee?>> UpdateAsync(UpdateEmployeeRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/employees/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Employee?>>()
               ?? new Response<Employee?>(null, 400, "Não foi possível alterar o funcionário!");
    }

    public async Task<Response<Employee?>> UpdateImageAsync(UpdateImageEmployeeRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/employees/{request.Id}/profile-image", request);
        return await result.Content.ReadFromJsonAsync<Response<Employee?>>()
               ?? new Response<Employee?>(null, 400, "Não foi possível alterar a foto do funcionário!");
    }

    public async Task<Response<Employee?>> DeleteAsync(DeleteEmployeeRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/employees/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Employee?>>()
            ?? new Response<Employee?>(null, 400, "Não foi possível Excluir o Funcionário!");
    }

    public async Task<Response<Employee?>> GenerateUniqueCodeAsync(GenerateUniqueEmployeeCodeRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/employees/generate-code/{request.EmployeeId}", request);
        return await result.Content.ReadFromJsonAsync<Response<Employee?>>()
               ?? new Response<Employee?>(null, 400, "Não foi possível alterar o funcionário!");
    }

    public async Task<Response<Employee?>> GetByIdAsync(GetEmployeeByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<Employee?>>($"v1/employees/{request.Id}")
            ?? new Response<Employee?>(null, 404, "Funcionário não Encontrado!");
    

    public async Task<PagedResponse<List<Employee>?>> GetAllAsync(GetAllEmployeesRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Employee>?>>($"v1/employees")
        ?? new PagedResponse<List<Employee>?>([], 404, "Nenhum Funcionário Encontrado!");
}