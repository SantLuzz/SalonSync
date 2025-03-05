using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class UpdateLastedProcedreClientEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/lasted-procedures/{id}", HandleAsync)
            .WithName("Client: Update a last procedure")
            .WithDescription("Update a last procedure the exists client.")
            .WithOrder(4)
            .Produces<Response<Client?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        UpdateLastedProcedureClientRequest request,
        ClaimsPrincipal user,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;
        
        var result = await handler.UpdateLastedProcedureAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}