using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class DeleteEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapDelete("/{id}", HandleAsync)
            .WithName("Employee: Delete")
            .WithDescription("Delete the exists employee.")
            .WithOrder(4)
            .Produces<Response<Employee?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new DeleteEmployeeRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Id = id
        };
        
        var result = await handler.DeleteAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}