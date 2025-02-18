using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Companies;

public class GenerateCodeByCompanyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/generate-code", HandleAsync)
            .WithName("Company: Generate Code")
            .WithDescription("Generate code for a Company")
            .WithOrder(3)
            .Produces<Response<Company?>>();

    private static async Task<IResult> HandleAsync(
        ICompanyHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GenerateCodeByCompanyRequest
        {
            UserId = user.Identity?.Name ?? string.Empty
        };
        
        var result = await handler.GenerateCodeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result);
    }
}