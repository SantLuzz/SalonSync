using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Clients;

public class GetAllClientsEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Client: Get All")
            .WithDescription("Get all Clients")
            .WithOrder(7)
            .Produces<PagedResponse<List<Client>?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        ClaimsPrincipal user,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllClientsRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize,
        };
        
        var result = await handler.GetAllAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}