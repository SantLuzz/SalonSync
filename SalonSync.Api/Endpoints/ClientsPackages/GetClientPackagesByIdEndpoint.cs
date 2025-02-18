using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class GetClientPackagesByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id}", HandlerAsync)
            .WithName("Client Package: Get by Id")
            .WithDescription("Get a existing package for the Client by Id.")
            .WithOrder(4)
            .Produces<Response<ClientPackage?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        long id)
    {
        var request = new GetClientPackageByIdRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Id = id
        };

        var result = await handler.GetByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}