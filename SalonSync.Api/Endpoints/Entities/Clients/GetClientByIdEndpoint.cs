using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class GetClientByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id}", HandleAsync)
            .WithName("Client: Get By Id")
            .WithDescription("Get Client by Id")
            .WithOrder(6)
            .Produces<Response<Client?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new GetClientByIdRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Id = id
        };
        
        var result = await handler.GetByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }
}