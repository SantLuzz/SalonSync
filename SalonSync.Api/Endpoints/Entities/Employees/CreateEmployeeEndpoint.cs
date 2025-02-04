using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class CreateEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/", HandleAsync)
            .WithName("Employee: Create")
            .WithDescription("Creates a new employee.")
            .WithOrder(1)
            .Produces<Response<Employee?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        CreateEmployeeRequest request,
        ClaimsPrincipal user)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;
        
        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Created($"/{result.Data?.Id}", result.Data)
            : TypedResults.BadRequest(result.Data);
    }
}