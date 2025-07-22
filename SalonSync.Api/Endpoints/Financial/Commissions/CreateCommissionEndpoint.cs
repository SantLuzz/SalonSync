using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.Commissions;

public class CreateCommissionEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Commission: Create")
            .WithDescription("Create a commission payment.")
            .WithOrder(1)
            .Produces<Response<Commission?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICommissionHandler handler,
        CreateCommissionRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}