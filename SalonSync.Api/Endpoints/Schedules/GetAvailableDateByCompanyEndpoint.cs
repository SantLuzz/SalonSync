using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Schedules;

public class GetAvailableDateByCompanyEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/available-dates/", HandlerAsync)
            .WithName("Schedules: Get available date by code company")
            .WithDescription("Get Available by company.")
            .WithOrder(2)
            .Produces<Response<List<AvailableDateDto>?>>();

    private static async Task<IResult> HandlerAsync(
        IAppointmentHandler handler,
        [FromQuery]Guid id)
    {
        var request = new GetAvailableDateByCompanyRequest()
        {
            Code = id,
        };

        var result = await handler.GetAvailableDatesByCompanyCodeAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}