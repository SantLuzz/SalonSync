using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Companies;

public class UpdateCompanyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/", HandleAsync)
            .WithName("Company: Update")
            .WithDescription("Update a exists company.")
            .WithOrder(2)
            .Produces<Response<Company?>>();

    private static async Task<IResult> HandleAsync(
        ICompanyHandler handler,
        ClaimsPrincipal user,
        UpdateCompanyRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        
        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}