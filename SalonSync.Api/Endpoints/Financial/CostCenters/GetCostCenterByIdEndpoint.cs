using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.CostCenters;

public class GetCostCenterByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id}", HandleAsync)
            .WithName("Cost Center: Get By Id")
            .WithDescription("Get cost center by Id")
            .WithOrder(6)
            .Produces<PagedResponse<List<CostCenter>?>>();

    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICostCenterHandler handler,
        long id)
    {
        var request = new GetCostCenterByIdRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };

        var result = await handler.GetByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}