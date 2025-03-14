using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Transactions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.Transactions;

public class GetTransactionsByPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Transaction: Get By Period")
            .WithDescription("Get Transaction by period.")
            .WithOrder(6)
            .Produces<PagedResponse<List<Transaction>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ITransactionHandler handler,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize)
    {
        var request = new GetTransactionsByPeriodRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
            

        var result = await handler.GetByPeriodAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
    
    
    
    
}