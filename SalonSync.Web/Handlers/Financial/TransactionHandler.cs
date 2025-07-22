using System.Net.Http.Json;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Transactions;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Financial;

public class TransactionHandler(IHttpClientFactory httpClientFactory) : ITransactionHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/transactions", request);
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
            ?? new Response<Transaction?>(null, 400, "Não foi possível criar a transação!");
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync($"v1/transactions/{request.Id}", request);
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
               ?? new Response<Transaction?>(null, 400, "Não foi possível alterar a transação!");
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        var result = await _httpClient.DeleteAsync($"v1/transactions/{request.Id}");
        return await result.Content.ReadFromJsonAsync<Response<Transaction?>>()
               ?? new Response<Transaction?>(null, 400, "Não foi possível alterar a transação!");
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
        => await _httpClient.GetFromJsonAsync<Response<Transaction?>>($"/v1/transactions/{request.Id}")
           ?? new Response<Transaction?>(null, 404, "Transação não encontrada!");

    public async Task<PagedResponse<List<Transaction>?>> GetByCostCenterAsync(
        GetTransactionsByCostCenterRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<Transaction>?>>(
               $"/v1/transactions/cost-center/{request.PageNumber}/{request.PageSize}")
           ?? new PagedResponse<List<Transaction>?>(null, 404,
               "Não foram encontrados transações para esse centro de custo!");

    public async Task<PagedResponse<List<Transaction>?>> GetByDayAsync(GetTransactionsByPeriodRequest request)
    {
        const string format = "yyyy-MM-dd";
        var startDate =  request.StartDate is not null
            ? request.StartDate.Value.ToString(format) 
            : DateTime.Now.GetFirstDay().ToString(format);
        
        var url = $"v1/transactions/by-date/{startDate}";
        
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Transaction>?>>(url)
               ?? new PagedResponse<List<Transaction>?>(null, 404,
                   "Não foram encontradas transações para o dia informado!");
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        const string format = "yyyy-MM-dd";
        var startDate =  request.StartDate is not null
            ? request.StartDate.Value.ToString(format) 
            : DateTime.Now.GetFirstDay().ToString(format);

        var endDate = request.EndDate is not null
            ? request.EndDate.Value.ToString(format)
            : DateTime.Now.GetLastDay().ToString(format);
        
        var url = $"v1/transactions?startDate={startDate}&endDate={endDate}";
        
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<Transaction>?>>(url)
            ?? new PagedResponse<List<Transaction>?>(null, 404,
                "Não foram encontradas transações para o período informado!");
    }
    
}