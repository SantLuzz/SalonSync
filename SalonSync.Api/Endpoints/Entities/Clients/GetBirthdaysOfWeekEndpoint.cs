using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class GetBirthdaysOfWeekEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/birthdays/", HandleAsync)
            .WithName("Client: Get birthdays of week")
            .WithDescription("Get birthdays of week")
            .WithOrder(9)
            .Produces<PagedResponse<List<ClientBirthdaysDTO>?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        ClaimsPrincipal user)
    {
        var request = new GetBirthdaysOfWeekRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
        };
        
        var result = await handler.GetBirthdaysOfWeek(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }
}