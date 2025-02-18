using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class DeleteClientPackageEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandlerAsync)
            .WithName("Client Package: Delete")
            .WithDescription("Delete a existing package for the Client.")
            .WithOrder(2)
            .Produces<Response<ClientPackage?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        long id)
    {
        var request = new DeleteClientPackageRequest
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