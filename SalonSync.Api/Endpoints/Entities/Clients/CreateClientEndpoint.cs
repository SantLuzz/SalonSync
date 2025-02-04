using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class CreateClientEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Client: Create")
            .WithDescription("Creates a new client.")
            .WithOrder(1)
            .Produces<Response<Client?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        CreateClientRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        
        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"/{result.Data?.Id}", result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}