using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Endpoints.Packages;
using SalonSync.Core;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Services;

public class GetServicesByPackageIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/package/{packageId:long}", HandlerAsync)
            .WithName("Service: Get All By Package Id")
            .WithDescription("Service: Get All By Package Id.")
            .WithOrder(6)
            .Produces<PagedResponse<List<Service>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IServiceHandler handler,
        long packageId,
        [FromQuery] int pageSize = Configuration.DefaultPageSize,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber)
    {
        var request = new GetServicesByPackageIdRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PackageId = packageId,
            PageNumber = pageNumber,
            PageSize = pageSize
        };

        var result = await handler.GetByPackageIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}