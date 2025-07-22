using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class InactiveClientIsActivePackageEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/", HandlerAsync)
            .WithName("Client Package: Inactive a Active Package")
            .WithDescription("Inactive a active package for the Client.")
            .WithOrder(6)
            .Produces<Response<ClientPackage?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        InactiveClientIsActivePackageRequest request)
    {
        
        request.UserId = user.Identity.Name ?? string.Empty;
        
        var result = await handler.InactiveActivePackageAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}