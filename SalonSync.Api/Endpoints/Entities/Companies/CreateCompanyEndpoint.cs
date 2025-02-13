using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Companies;

public class CreateCompanyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Company: Create")
            .WithDescription("Creates a new company.")
            .WithOrder(1)
            .Produces<Response<Company?>>();

    private static async Task<IResult> HandleAsync(
        ICompanyHandler handler,
        CreateCompanyRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        
        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"/{result.Data?.Id}", result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}