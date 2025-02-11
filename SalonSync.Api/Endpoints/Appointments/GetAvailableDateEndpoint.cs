using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class GetAvailableDateEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/available-date/", HandlerAsync)
            .WithName("Appointments: Get Available Date")
            .WithDescription("Get Available Date.")
            .WithOrder(2)
            .Produces<Response<AvailableDateDto?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAppointmentHandler handler)
    {
        var request = new GetAvailableDateRequest
        {
            UserId = user.Identity?.Name ?? string.Empty,
        };

        var result = await handler.GetAvailableDatesAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}