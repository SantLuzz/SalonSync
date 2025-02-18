using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.PackagesServices;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.PackagesServices;

public class UpdatePackageServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandlerAsync)
            .WithName("Package Service: Update")
            .WithDescription("Update a existing relation package service.")
            .WithOrder(2)
            .Produces<Response<PackageService?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPackageServiceHandler handler,
        UpdatePackageServiceRequest request,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;

        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}