using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class ReportSessionUsedByClientEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/report-use/{packageId:long}", HandlerAsync)
            .WithName("Client Package: Report use")
            .WithDescription("Report use in package")
            .WithOrder(9)
            .Produces<Response<ClientPackage?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IClientPackageHandler handler,
        long packageId,
        ReportSessionUsedByClientRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.PackageId = packageId;
        
        var result = await handler.ReportSessionUsedAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}