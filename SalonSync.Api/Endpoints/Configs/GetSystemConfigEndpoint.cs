using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers;
using SalonSync.Core.Models;
using SalonSync.Core.Requests.Configs;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Configs;

public class GetSystemConfigEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Configs: Get by Client")
            .WithDescription("Get configs by client")
            .WithOrder(2)
            .Produces<Response<SystemConfig?>>();

    private static async Task<IResult> HandleAsync(
        ISystemConfigHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetSystemConfigRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
        };
        
        var result = await handler.GetConfigAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }
}