using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.CostCenters;

public class CreateCostCenterEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Cost Center: Create")
            .WithDescription("Creates a new cost center.")
            .WithOrder(1)
            .Produces<Response<CostCenter?>>();

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICostCenterHandler handler,
        CreateCostCenterRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"/{result.Data?.Id}", result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}