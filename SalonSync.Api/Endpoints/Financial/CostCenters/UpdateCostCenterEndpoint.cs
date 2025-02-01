using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Financial.CostCenters;

public class UpdateCostCenterEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}", HandleAsync)
            .WithName("Cost Center: Update")
            .WithDescription("Update a existing cost center.")
            .WithOrder(2)
            .Produces<Response<CostCenter?>>();
    
    private static async Task<IResult> HandleAsync(
        ClaimsPrincipal user,
        ICostCenterHandler handler,
        UpdateCostCenterRequest request,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
       
        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result.Data)
            : TypedResults.NotFound(result);
    }
}