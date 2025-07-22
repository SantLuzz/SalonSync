using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.AccountsPayable;

public class DeleteAccountPayableEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandlerAsync)
            .WithName("Account Payable: Delete")
            .WithDescription("Delete a Account Payable.")
            .WithOrder(3)
            .Produces<Response<AccountPayable?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAccountPayableHandler handler,
        long id)
    {
        var request = new DeleteAccountPayableRequest
        {
            Id = id,
            UserId = user.Identity?.Name ?? string.Empty
        };
        
        var result = await handler.DeleteAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}