using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class DeleteClientEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandleAsync)
            .WithName("Client: Delete")
            .WithDescription("Delete the exists client.")
            .WithOrder(5)
            .Produces<Response<Client?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new DeleteClientRequest
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