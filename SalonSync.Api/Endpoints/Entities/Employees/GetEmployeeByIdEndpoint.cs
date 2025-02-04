using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class GetEmployeeByIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/{id}", HandleAsync)
            .WithName("Employee: Get By Id")
            .WithDescription("Get Employee by ID")
            .WithOrder(5)
            .Produces<Response<Employee?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new GetEmployeeByIdRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
            Id = id
        };
        
        var result = await handler.GetByIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}