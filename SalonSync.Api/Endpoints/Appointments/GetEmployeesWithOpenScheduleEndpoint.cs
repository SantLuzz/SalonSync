using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class GetEmployeesWithOpenScheduleEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/employees-open-schedule/{date:datetime}", HandlerAsync)
            .WithName("Appointments: Get employees with open schedule")
            .WithDescription("Get employees with open schedule.")
            .WithOrder(4)
            .Produces<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAppointmentHandler handler,
        DateTime date)
    {
        var request = new GetEmployeesWithOpenScheduleRequest()
        {
            Date = date,
            UserId = user.Identity?.Name ?? string.Empty,
        };
        
        var result = await handler.GetEmployeesWithOpenScheduleAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}