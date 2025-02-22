using System.Net.Http.Json;
using System.Text;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers;
using SalonSync.Core.Requests.Account;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers
{
    public class AccountHandler(IHttpClientFactory httpClientFactory) : IAccountHandler
    {
        private readonly HttpClient _client = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);

        public async Task<Response<string>> LoginAsync(LoginRequest request)
        {
            var result = await _client.PostAsJsonAsync("v1/identity/login-user", request);
            return result.IsSuccessStatusCode
                ? new Response<string>("Login realizado com sucesso!", 200, "Login realizado com sucesso!")
                : new Response<string>("", (int)result.StatusCode, "Não foi possível realizar o login!");
        }

        public async Task<UserInfoDto?> GetManageInfo()
            => await _client.GetFromJsonAsync<UserInfoDto>("v1/identity/manage-info");

        public async Task LogoutAsync()
        {
            var emptyContent = new StringContent("{}", Encoding.UTF8,  "application/json");
            await _client.PostAsJsonAsync("v1/identity/logout", emptyContent);
        }

        public async Task<Response<string>> RegisterAsync(RegisterRequest request)
        {
            var result = await _client.PostAsJsonAsync("v1/identity/register-user", request);
            return result.IsSuccessStatusCode
                ? new Response<string>("Cadastro realizado com sucesso!", 201, "Cadastro realizado com sucesso!")
                : new Response<string>("", (int)result.StatusCode, "Não foi possível realizar o seu cadastro!");
        }
    }
}
