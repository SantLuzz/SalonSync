using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Identity;

public class GetUserInfoEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/manage-info", Handle)
            .Produces<UserInfoDto?>();

    private static IResult Handle(HttpContext httpContext)
    {
        if (!httpContext.User.Identity.IsAuthenticated)
            return TypedResults.Unauthorized();
        

        // Obtém os claims do usuário
        var userClaims = httpContext.User.Claims;

        // Pega os roles do usuário
        var roles = httpContext.User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();

        // Cria a resposta com as informações do usuário
        var userInfo = new UserInfoDto
        {
            Email = httpContext.User.FindFirst("CompanyEmail")?.Value,
            Name = httpContext.User.FindFirst("CompanyName")?.Value,
            Roles = roles,
            Claims = userClaims.Select(c => new ClaimDto
            {
                Type = c.Type,
                Value = c.Value,
            }).ToList()
        };

        // Retorna as informações do usuário
        return TypedResults.Ok(userInfo);
    }
    
    
}