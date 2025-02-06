using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Services;

public class DeleteServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandlerAsync)
            .WithName("Service: Delete")
            .WithDescription("Delete a service.")
            .WithOrder(3)
            .Produces<Response<Service?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IServiceHandler handler,
        long id)
    {
        var request = new DeleteServiceRequest
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