using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class UpdateImageEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/{id}/profile-image", HandleAsync)
            .WithName("Employee: Update a Image")
            .WithDescription("Update a image the exists employee.")
            .WithOrder(3)
            .Produces<Response<Employee?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        UpdateImageEmployeeRequest request,
        ClaimsPrincipal user,
        long id)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        request.Id = id;
        
        var result = await handler.UpdateImageAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}