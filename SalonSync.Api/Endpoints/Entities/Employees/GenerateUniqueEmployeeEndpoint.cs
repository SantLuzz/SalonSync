using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class GenerateUniqueEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/generate-code/{id}", HandleAsync)
            .WithName("Employee: Generate code for employee id")
            .WithDescription("Generate code for employee id")
            .WithOrder(5)
            .Produces<Response<Employee?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        ClaimsPrincipal user,
        long id)
    {
        var request = new GenerateUniqueEmployeeCodeRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            EmployeeId = id
        };
        
        var result = await handler.GenerateUniqueCodeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}