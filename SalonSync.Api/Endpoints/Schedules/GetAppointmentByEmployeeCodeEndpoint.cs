using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Schedules;

public class GetAppointmentByEmployeeCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/employees-appointments/", HandlerAsync)
            .WithName("Schedules: Get Appointments by code employee")
            .WithDescription("Get Schedules by employee.")
            .WithOrder(1)
            .Produces<PagedResponse<List<AppointmentDto>?>>();

    private static async Task<IResult> HandlerAsync(
        IAppointmentHandler handler,
        [FromQuery]Guid id,
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