using Microsoft.AspNetCore.Identity;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Models;
using SalonSync.Core;
using SalonSync.Core.Requests.Account;

namespace SalonSync.Api.Endpoints.Identity;

public class RegisterUserEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/register-user", Handle)
            .Produces<RegisterResponse>()
            .WithName("RegisterUser");
    

    private static async Task<IResult> Handle(RegisterRequest request, 
        UserManager<User> userManager, 
        RoleManager<IdentityRole<long>> roleManager)
    {
        // Verificar se o role "CompanyUser" já existe
        var roleExist = await roleManager.RoleExistsAsync(Configuration.CompanyUserRole);
        if (!roleExist)
        {
            // Criar o role "CompanyUser" se não existir
            var role = new IdentityRole<long> { Name = Configuration.CompanyUserRole };
            await roleManager.CreateAsync(role);
        }

        // Criar o usuário
        var user = new User { UserName = request.Email, Email = request.Email };
        var result = await userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(user, Configuration.CompanyUserRole);

            return Results.Ok(new RegisterResponse
            {
                Message = "Cadastro realizado com sucesso!",
                Success = true
            });
        }
        else
        {
            return Results.BadRequest(new RegisterResponse
            {
                Message = "Erro ao realizar o cadastro: " + string.Join(", ", result.Errors.Select(e => e.Description)),
                Success = false
            });
        }
    }

    private class RegisterResponse
    {
        public string Message { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}

