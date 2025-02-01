using SalonSync.Core.Requests.Account;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers
{
    public interface IAccountHandler
    {
        Task<Response<string>> LoginAsync(LoginRequest request);
        Task<Response<string>> RegisterAsync(RegisterRequest request);
        Task LogoutAsync();
    }
}
