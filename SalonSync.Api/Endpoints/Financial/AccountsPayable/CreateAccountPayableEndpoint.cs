using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.AccountsPayable;

public class CreateAccountPayableEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandlerAsync)
            .WithName("Account Payable: Create")
            .WithDescription("Creates a new Account Payable.")
            .WithOrder(1)
            .Produces<Response<AccountPayable?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAccountPayableHandler handler,
        CreateAccountPayableRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}