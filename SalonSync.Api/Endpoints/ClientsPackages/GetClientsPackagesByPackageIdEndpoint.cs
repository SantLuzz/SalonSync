using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class GetClientsPackagesByPackageIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/packages/{packageId}", HandlerAsync)
            .WithName("Client Package: Get all by Package Id")
            .WithDescription("Get et all by Package Id.")
            .WithOrder(8)
            .Produces<Response<ClientPackageDto?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        long packageId)
    {
        var request = new GetClientsPackagesByPackageIdRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PackageId = packageId
        };

        var result = await handler.GetByPackageIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}