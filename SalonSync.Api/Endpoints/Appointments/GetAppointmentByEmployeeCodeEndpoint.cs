using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class GetAppointmentByEmployeeCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/appointments/{id:guid}", HandlerAsync)
            .WithName("Appointments: Get Appointments by code employee")
            .WithDescription("Get Appointments by employee.")
            .WithOrder(1)
            .Produces<PagedResponse<List<Appointment>?>>();

    private static async Task<IResult> HandlerAsync(
        IAppointmentHandler handler,
        Guid id,
        [FromQuery] DateTime date)
    {
        var request = new GetAppointmentByEmployeeCodeRequest()
        {
            Code = id,
            Date = date
        };

        var result = await handler.GetByEmployeeCodeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}