using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Services;

public class GetAllServicesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Service: Get All")
            .WithDescription("Get all services.")
            .WithOrder(5)
            .Produces<PagedResponse<List<Service>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IServiceHandler handler,
        [FromQuery] int pageSize = Configuration.DefaultPageSize,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber)
    {
        var request = new GetAllServicesRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await handler.GetAllAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}