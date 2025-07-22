using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.Commissions;

public class DeleteCommissionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandlerAsync)
            .WithName("Commission: Delete")
            .WithDescription("Delete a commission payment.")
            .WithOrder(2)
            .Produces<Response<Commission?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICommissionHandler handler,
        long id)
    {
        var request = new DeleteCommissionRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Id = id
        };
        
        var result = await handler.DeleteAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}