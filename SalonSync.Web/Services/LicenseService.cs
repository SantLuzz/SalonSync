using Microsoft.AspNetCore.Components;
using SalonSync.Core;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Web.Security;

namespace SalonSync.Web.Services;

public class LicenseService(ICookieAuthenticationStateProvider authenticationStateProvider,
    ILicenseHandler licenseHandler,
    NavigationManager navigationManager)
{
    public async Task<bool> ValidateLicenseAsync()
    {
        // ✅ Verifica se o usuário está autenticado
        if (!await authenticationStateProvider.CheckAuthenticatedAsync())
            return false;

        var auth = await authenticationStateProvider.GetAuthenticationStateAsync();

        // ✅ Super Admin não precisa de validação de licença
        if (auth.User.IsInRole(Configuration.SuperAdminRole))
            return true;

        // 🔍 Obtém a data de expiração da licença do cookie
        var dataExpiracao = auth.User.FindFirst("CompanyLicense")?.Value;

        // ✅ Se a licença for válida no cookie, retorna
        if (!string.IsNullOrWhiteSpace(dataExpiracao) && DateTime.TryParse(dataExpiracao, out var expirationDate))
        {
            if (expirationDate >= DateTime.Today)
                return true; // ✅ Licença válida
        }

        // 🔄 Se a licença não for válida ou não existir, tenta buscar no serviço
        var isValidLicense = await licenseHandler.CheckAsync(auth.User.Identity?.Name ?? string.Empty);

        if (isValidLicense) return true;
        
        Console.WriteLine("Redirecionando para a página de licença expirada...");
        _ = Task.Run(() => navigationManager.NavigateTo("/license-expired", true));
        return false;
    }
}