using System.Net.Http.Json;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Financial;

public class AccountPayableHandler(IHttpClientFactory httpClientFactory) : IAccountPayableHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<Response<AccountPayable?>> CreateAsync(CreateAccountPayableRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/account-payable", request);
        return await result.Content.ReadFromJsonAsync<Response<AccountPayable?>>()
            ?? new Response<AccountPayable?>(null, 400, "Não foi possível criar a conta a pagar!");
    }

    public async Task<Response<AccountPayable?>> UpdateAsync(UpdateAccountPayableRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/account-payable/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<AccountPayable?>>()
               ?? new Response<AccountPayable?>(null, 400, "Não foi possível editar a conta a pagar!");
    }

    public async Task<Response<AccountPayable?>> DeleteAsync(DeleteAccountPayableRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/account-payable/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<AccountPayable?>>()
               ?? new Response<AccountPayable?>(null, 400, "Não foi possível editar a conta a pagar!");
    }

    public async Task<Response<AccountPayable?>> CloseAsync(CloseAccountPayableRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/account-payable/{request.Id}/close", request);
        return await result.Content.ReadFromJsonAsync<Response<AccountPayable?>>()
               ?? new Response<AccountPayable?>(null, 400, "Não foi possível pagar a conta a pagar!");
    }

    public async Task<Response<AccountPayable?>> GetByIdAsync(GetAccountPayableByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<AccountPayable?>>($"v1/account-payable/{request.Id}")
           ?? new Response<AccountPayable?>(null, 404, "Conta não encontrada!");

    public async Task<PagedResponse<List<AccountPayable>?>> GetByPeriodAsync(GetAccountPayableByPeriodRequest request)
    {
        const string format = "yyyy-MM-dd";
        var startDate =  request.StartDate is not null
            ? request.StartDate.Value.ToString(format) 
            : DateTime.Now.GetFirstDay().ToString(format);

        var endDate = request.EndDate is not null
            ? request.EndDate.Value.ToString(format)
            : DateTime.Now.GetLastDay().ToString(format);
        
        var url = $"v1/account-payable?startDate={startDate}&endDate={endDate}&accountStatus={request.AccountStatus}";

        return await _httpClient.GetFromJsonAsync<PagedResponse<List<AccountPayable>?>>(url)
               ?? new PagedResponse<List<AccountPayable>?>(null, 404, "Conta não encontrada!");
    }
}