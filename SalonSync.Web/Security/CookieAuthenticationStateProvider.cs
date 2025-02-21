using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers;

namespace SalonSync.Web.Security
{
    public class CookieAuthenticationStateProvider(IAccountHandler handler)
        : AuthenticationStateProvider, ICookieAuthenticationStateProvider
    {
        private bool _isAuthenticated = false;

        public async Task<bool> CheckAuthenticatedAsync()
        {
            await GetAuthenticationStateAsync();
                        return _isAuthenticated;
        }

        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            _isAuthenticated = false;
            var user = new ClaimsPrincipal(new ClaimsIdentity());

            // Obtém o usuário através do cookie de identidade
            var userInfo = await GetUserInfo();

            if (userInfo == null)
                return new AuthenticationState(user); // Retorna um usuário não autenticado

            // Cria a lista de claims a partir do usuário
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userInfo.Name),
                new Claim(ClaimTypes.Email, userInfo.Email)
            };

            // Adiciona os claims adicionais, incluindo roles e outros claims personalizados
            claims.AddRange(userInfo.Claims.Select(c => new Claim(c.Type, c.Value)));
            claims.AddRange(userInfo.Roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var identity = new ClaimsIdentity(claims, nameof(CookieAuthenticationStateProvider));
            user = new ClaimsPrincipal(identity);

            _isAuthenticated = true;
            return new AuthenticationState(user);
        }

        // Método para consumir o endpoint e obter as informações do usuário
        private async Task<UserInfoDto?> GetUserInfo()
        {
            try
            {
                return await handler.GetManageInfo();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void NotifyAuthenticationStateChanged()
            => base.NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
