using Microsoft.AspNetCore.Components;
using SalonSync.Core.Handlers;
using SalonSync.Web.Security;

namespace SalonSync.Web.Services;

public class LogoutService(IAccountHandler handler, 
    NavigationManager navigationManager, 
    ICookieAuthenticationStateProvider authenticationStateProvider)
{
    
    public async Task<bool> Logout()
    {
        if (await authenticationStateProvider.CheckAuthenticatedAsync())
        {
            await handler.LogoutAsync();
            await authenticationStateProvider.GetAuthenticationStateAsync();
            authenticationStateProvider.NotifyAuthenticationStateChanged();
        }

        return true;
    }
}