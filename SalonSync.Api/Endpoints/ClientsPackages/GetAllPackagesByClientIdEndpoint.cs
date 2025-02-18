using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class GetAllPackagesByClientIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/client/{id}", HandlerAsync)
            .WithName("Client Package: Get All by Client Id")
            .WithDescription("Get a all packages for the Client.")
            .WithOrder(5)
            .Produces<PagedResponse<List<ClientPackage>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        long id,
        [FromQuery] int pageSize = Configuration.DefaultPageSize,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber)
    {
        var request = new GetAllPackagesByClientIdRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            ClientId = id
        };

        var result = await handler.GetAllByClientIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}