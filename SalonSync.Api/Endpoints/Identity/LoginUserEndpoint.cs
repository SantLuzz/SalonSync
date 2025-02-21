using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Models;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Account;
using SalonSync.Core.Requests.Entities.Companies;

namespace SalonSync.Api.Endpoints.Identity;

public class LoginUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/login-user", Handle)
            .Produces<LoginResponse>()
            .WithName("LoginUser");
    

    private static async Task<IResult> Handle(LoginRequest request,
        UserManager<User> userManager, 
        SignInManager<User> signInManager,
        ICompanyHandler handler,
        HttpContext context)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        if(user is null || !await userManager.CheckPasswordAsync(user, request.Password))
            return TypedResults.BadRequest(new LoginResponse("Credenciais inválidas!", false));
        
        var company = await handler.GetAsync(new GetCompanyRequest{UserId = request.Email });
        if (!company.IsSuccess) 
            return TypedResults.BadRequest(new LoginResponse("Empresa não encontrada!", false));
        
        if(string.IsNullOrWhiteSpace(company.Data?.LicenseEndDate))
            return TypedResults.BadRequest(new LoginResponse("Licença não encontrada!", false));
        
        var claims = await userManager.GetClaimsAsync(user);
        var roles = await userManager.GetRolesAsync(user);

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));
        
        claims.Add(new Claim("CompanyId", company.Data.Id.ToString()));
        claims.Add(new Claim("CompanyName", company.Data.Name));
        claims.Add(new Claim("CompanyEmail", company.Data.UserId));
        claims.Add(new Claim("CompanyCnpj", company.Data.Document ?? string.Empty));
        claims.Add(new Claim("CompanyLicense", company.Data.LicenseEndDate));
        
        var identity = new ClaimsIdentity(claims,  IdentityConstants.ApplicationScheme);
        var principal = new ClaimsPrincipal(identity);
        
        await context.SignInAsync(IdentityConstants.ApplicationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true, // Mantém o usuário logado mesmo após fechar o navegador
            ExpiresUtc = DateTime.Now.AddHours(DefineCookieExpiration()) // Define tempo de expiração do cookie
        });
        
        return TypedResults.Ok(new LoginResponse("Login efetuado com sucesso!", true));
    }

    private static int DefineCookieExpiration()
    {
        var currentDate = DateTime.Now;
        var midnight = currentDate.Date.AddDays(1);
        
        if (currentDate > midnight)
            midnight = currentDate.Date.AddDays(1);

        var time = (midnight - currentDate);
        
        return time.Hours;
    }
    
    private class LoginResponse(string message, bool success)
    {
        public string Message { get; set; } = message;
        public bool Success { get; set; } = success;
    }
}