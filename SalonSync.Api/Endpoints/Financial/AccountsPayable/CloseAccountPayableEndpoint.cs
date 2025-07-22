using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.AccountsPayable;

public class CloseAccountPayableEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}/close", HandlerAsync)
            .WithName("Account Payable: Close")
            .WithDescription("Close a Account Payable.")
            .WithOrder(4)
            .Produces<Response<AccountPayable?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAccountPayableHandler handler,
        CloseAccountPayableRequest request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CloseAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}