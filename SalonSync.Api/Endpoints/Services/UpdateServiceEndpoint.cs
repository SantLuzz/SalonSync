using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Services;

public class UpdateServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandlerAsync)
            .WithName("Service: Update")
            .WithDescription("Update a existing service.")
            .WithOrder(2)
            .Produces<Response<Service?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IServiceHandler handler,
        UpdateServiceRequest request,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;

        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}