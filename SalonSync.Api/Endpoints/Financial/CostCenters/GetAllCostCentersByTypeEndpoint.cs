using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.CostCenters;

public class GetAllCostCentersByTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/types", HandleAsync)
            .WithName("Cost Center: Get by Type")
            .WithDescription("Get all cost center by type")
            .WithOrder(4)
            .Produces<PagedResponse<List<CostCenter>?>>();
    
    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICostCenterHandler handler,
        [FromQuery] ECostCenterType type = ECostCenterType.Income,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllCostCentersByTypeRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Type = type,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
       
        var result = await handler.GetAllByTypeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}