using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class UpdateClientEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandleAsync)
            .WithName("Client: Update")
            .WithDescription("Update a exists client.")
            .WithOrder(2)
            .Produces<Response<Client?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        UpdateClientRequest request,
        ClaimsPrincipal user,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;
        
        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }

    
}