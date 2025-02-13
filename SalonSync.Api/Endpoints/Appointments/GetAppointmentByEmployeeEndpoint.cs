using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class GetAppointmentByEmployeeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
    => app.MapGet("/employees-appointments/{id:long}", HandlerAsync)
            .WithName("Appointments: Get Appointments by employee")
            .WithDescription("Get Appointments by employee.")
            .WithOrder(6)
            .Produces<PagedResponse<List<Appointment>?>>();

        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IAppointmentHandler handler,
            long id,
            [FromQuery] DateTime date)
        {
            
            
            var request = new GetAppointmentByEmployeeRequest()
            {
                UserId = user.Identity?.Name ?? string.Empty,
                EmployeeId = id,
                Date = date
            };

            var result = await handler.GetByEmployeeAsync(request);
            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.NotFound(result);
        }
}