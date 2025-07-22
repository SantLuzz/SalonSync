using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.Commissions;

public class GetUnpaidCommissionInPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Commission: Get the unpaid Commissions in the period")
            .WithDescription("Get the unpaid Commissions in the period.")
            .WithOrder(2)
            .Produces<PagedResponse<List<CommissionDto>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ICommissionHandler handler,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var request = new GetUnpaidCommissionInPeriodRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            StartDate = startDate,
            EndDate = endDate
        };
        
        var result = await handler.GetUnpaidAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}