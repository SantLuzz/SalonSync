using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Services;

public class CreateServiceEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Service: Create")
            .WithDescription("Creates a new service.")
            .WithOrder(1)
            .Produces<Response<Service?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IServiceHandler handler,
        CreateServiceRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"/{result.Data?.Id}",result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}