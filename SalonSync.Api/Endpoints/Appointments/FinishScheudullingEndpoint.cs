using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Appointments;

public class FinishScheudullingEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/finish", HandlerAsync)
            .WithName("Appointments: Finish Scheudules")
            .WithDescription("Finish Scheudules.")
            .WithOrder(8)
            .Produces<Response<SchedullingClosedDTO?>>();

    private static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        IAppointmentHandler handler,
        FinishSchedulingRequest request)
    {
        request.UserId = user.Identity?.Name ?? string.Empty;

        var result = await handler.FinishAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}