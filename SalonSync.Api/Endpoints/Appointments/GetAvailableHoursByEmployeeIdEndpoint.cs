using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class GetAvailableHoursByEmployeeIdEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/employees-available-hours/", HandlerAsync)
            .WithName("Appointments: Get available hours for employee id")
            .WithDescription("Get available hours for employee id.")
            .WithOrder(4)
            .Produces<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAppointmentHandler handler,
        [FromQuery]DateTime date,
        [FromQuery]long employeeId,
        [FromQuery]long serviceId)
    {
        var request = new GetAvailableHoursByServicesAndEmployeeRequest()
        {
            Date = date,
            UserId = user.Identity?.Name ?? string.Empty,
            EmployeeId = employeeId,
            ServiceId = serviceId
        };
        
        var result = await handler.GetAvailablesHoursByEmployeeIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}