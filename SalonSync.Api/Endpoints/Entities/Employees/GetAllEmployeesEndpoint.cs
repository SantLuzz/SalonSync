using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Entities.Employees;

public class GetAllEmployeesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandleAsync)
            .WithName("Employee: Get All")
            .WithDescription("Get All Employees")
            .WithOrder(6)
            .Produces<PagedResponse<List<Employee>?>>();

    private static async Task<IResult> HandleAsync(
        IEmployeeHandler handler,
        ClaimsPrincipal user,
        [FromQuery]int pageSize = Configuration.DefaultPageSize,
        [FromQuery]int pageNumber = Configuration.DefaultPageNumber)
    {
        var request = new GetAllEmployeesRequest()
        {
            UserId = user.Identity?.Name ?? string.Empty,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        
        var result = await handler.GetAllAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}