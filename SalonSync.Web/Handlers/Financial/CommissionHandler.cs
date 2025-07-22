using SalonSync.Core.Common.Extensions;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;
using System.Net.Http.Json;

namespace SalonSync.Web.Handlers.Financial
{
    public class CommissionHandler(IHttpClientFactory httpClientFactory) : ICommissionHandler
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
        public async Task<Response<Commission?>> CreateAsync(CreateCommissionRequest request)
        {
            var result = await _httpClient.PostAsJsonAsync("v1/commission", request);
            return await result.Content.ReadFromJsonAsync<Response<Commission?>>()
                ?? new Response<Commission?>(null, 400, "Não foi possível lançar a comissão!");
        }

        public async Task<Response<Commission?>> DeleteAsync(DeleteCommissionRequest request)
        {
            var result = await _httpClient.DeleteAsync($"v1/commission/{request.Id}");
            return await result.Content.ReadFromJsonAsync<Response<Commission?>>()
                   ?? new Response<Commission?>(null, 404, "Não foi possível deletar a comissão!");
        }

        public async Task<PagedResponse<List<UnpaidCommissionDto>?>> GetUnpaidAsync(GetUnpaidCommissionInPeriodRequest request)
        {
            const string format = "yyyy-MM-dd";
            var startDate = request.StartDate is not null
                ? request.StartDate.Value.ToString(format)
                : DateTime.Now.GetFirstDay().ToString(format);

            var endDate = request.EndDate is not null
                ? request.EndDate.Value.ToString(format)
                : DateTime.Now.GetLastDay().ToString(format);

            var url = $"v1/commission?startDate={startDate}&endDate={endDate}";

            return await _httpClient.GetFromJsonAsync<PagedResponse<List<UnpaidCommissionDto>?>>(url)
                ?? new PagedResponse<List<UnpaidCommissionDto>?>(null, 404,
                    "Não foram encontradas comissões a serem pagas no período!");
        }
    }
}
