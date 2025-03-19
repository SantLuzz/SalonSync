using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.AccountsPayable;

public class UpdateAccountPayableEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandlerAsync)
            .WithName("Account Payable: Edit")
            .WithDescription("Edit a Account Payable.")
            .WithOrder(2)
            .Produces<Response<AccountPayable?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAccountPayableHandler handler,
        UpdateAccountPayableRequest request,
        long id)
    {
        request.Id = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}