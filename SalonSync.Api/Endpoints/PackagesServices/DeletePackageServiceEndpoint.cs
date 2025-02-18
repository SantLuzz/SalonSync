using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.PackagesServices;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.PackagesServices;

public class DeletePackageServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandlerAsync)
            .WithName("Package Service: Delete")
            .WithDescription("Delete a existing relation package service.")
            .WithOrder(3)
            .Produces<Response<PackageService?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPackageServiceHandler handler,
        long id)
    {
        var request = new DeletePackageServiceRequest
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