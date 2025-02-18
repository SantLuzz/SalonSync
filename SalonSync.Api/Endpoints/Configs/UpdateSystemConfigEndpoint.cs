using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers;
using SalonSync.Core.Models;
using SalonSync.Core.Requests.Configs;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Configs;

public class UpdateSystemConfigEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/", HandleAsync)
            .WithName("Configs: Update by Client")
            .WithDescription("update configs by client")
            .WithOrder(1)
            .Produces<Response<SystemConfig?>>();

    private static async Task<IResult> HandleAsync(
        ISystemConfigHandler handler,
        ClaimsPrincipal user,
        UpdateSystemConfigRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        
        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }
}