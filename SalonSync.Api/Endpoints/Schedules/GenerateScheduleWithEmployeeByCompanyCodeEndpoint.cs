using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Schedules;

public class GenerateScheduleWithEmployeeByCompanyCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPost("/generate/", HandlerAsync)
            .WithName("Schedules: Open Appointment")
            .WithDescription("Open Appointment.")
            .WithOrder(3)
            .AllowAnonymous()
            .Produces<Response<AppointmentCreatedDto?>>();

    private static async Task<IResult> HandlerAsync(
        [FromQuery]Guid id,
        IAppointmentHandler handler,
        ICompanyHandler companyHandler,
        GenerateScheduleWithEmployeeRequest request)
    {
        Company? company = null;
        try
        {
            company = await companyHandler.GetByCodeAsync(id);
            if(company is null)
                return TypedResults.NotFound(new Response<AppointmentCreatedDto>(null,
                    404, "Empresa não encontrada!"));
        }
        catch
        {
            return TypedResults.NotFound(new Response<AppointmentCreatedDto>(null,
                404, "Não foi possível validar a empresa!"));
        }

        request.UserId = company.UserId;

        var result = await handler.CreateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}