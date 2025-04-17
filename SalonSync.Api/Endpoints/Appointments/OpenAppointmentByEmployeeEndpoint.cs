using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class OpenAppointmentByEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/{id}/open", HandlerAsync)
            .WithName("Appointments: Open Scheudules")
            .WithDescription("Open Scheudules for the employee.")
            .WithOrder(1)
            .Produces<Response<EmployeeAvailabilityScheduleDTO?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAppointmentHandler handler,
        long id,
        OpenAppointmentByEmployeeRequest request)
    {
        request.EmployeeId = id;
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.OpenByEmployeeIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.BadRequest(result);
    }
}