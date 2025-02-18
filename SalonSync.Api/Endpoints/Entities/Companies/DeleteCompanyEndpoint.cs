using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Companies;

public class DeleteCompanyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/", HandleAsync)
            .WithName("Company: Delete")
            .WithDescription("Delete a exists company.")
            .WithOrder(4)
            .Produces<Response<Company?>>();

    private static async Task<IResult> HandleAsync(
        ICompanyHandler handler,
        ClaimsPrincipal user)
    {
        var request = new DeleteCompanyRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
        };
        
        var result = await handler.DeleteAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result);
    }
}