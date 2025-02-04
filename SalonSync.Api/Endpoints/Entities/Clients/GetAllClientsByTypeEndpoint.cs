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

public class GetAllClientsByTypeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/types/", HandleAsync)
            .WithName("Client: Get All by Type")
            .WithDescription("Get all Client by type")
            .WithOrder(8)
            .Produces<PagedResponse<List<Client>?>>();

    private static async Task<IResult> HandleAsync(
        IClientHandler handler,
        ClaimsPrincipal user,
        [FromQuery] EClientType type,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetAllClientsByTypeRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize,
            Type = type 
        };
        
        var result = await handler.GetAllByTypeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result.Data);
    }
}