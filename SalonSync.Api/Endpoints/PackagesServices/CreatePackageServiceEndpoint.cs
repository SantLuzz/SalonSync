using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Requests.PackagesServices;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.PackagesServices;

public class CreatePackageServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Package Service: Create")
            .WithDescription("Creates a new relation package service.")
            .WithOrder(1)
            .Produces<Response<PackageDto?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IPackageServiceHandler handler,
        CreatePackageServiceRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}