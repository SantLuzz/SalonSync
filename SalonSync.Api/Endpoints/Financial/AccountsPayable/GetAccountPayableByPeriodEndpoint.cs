using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.AccountsPayable;

public class GetAccountPayableByPeriodEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Account Payable: Get By period")
            .WithDescription("Get Account Payable By period.")
            .WithOrder(6)
            .Produces<PagedResponse<List<AccountPayable>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAccountPayableHandler handler,
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null,
        [FromQuery] int pageNumber = Configuration.DefaultPageNumber,
        [FromQuery] int pageSize = Configuration.DefaultPageSize,
        [FromQuery] EAccountStatus accountStatus = EAccountStatus.All)
    {
        var request = new GetAccountPayableByPeriodRequest
        {
            StartDate = startDate,
            EndDate = endDate,
            PageNumber = pageNumber,
            PageSize = pageSize,
            UserId = user.Identity?.Name ?? string.Empty,
            AccountStatus = accountStatus
        };

        var result = await handler.GetByPeriodAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}