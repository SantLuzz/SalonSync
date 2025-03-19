using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Transactions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.Transactions;

public class GetTransactionsByDateEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/by-date/{date:datetime}", HandlerAsync)
            .WithName("Transaction: Get By transaction Date")
            .WithDescription("Get Transaction by transaction Date.")
            .WithOrder(7)
            .Produces<PagedResponse<List<Transaction>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ITransactionHandler handler,
        DateTime date)
    {
        var request = new GetTransactionsByPeriodRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            StartDate = date,
        };
            

        var result = await handler.GetByDayAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}