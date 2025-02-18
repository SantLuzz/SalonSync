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

public class GetEmployeesWithOpenScheduleByCompanyCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/employees-open-schedule/", HandlerAsync)
            .WithName("Schedules: Get employees with open schedule")
            .WithDescription("Get employees with open schedule.")
            .WithOrder(4)
            .Produces<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>>();

    private static async Task<IResult> HandlerAsync(
        IAppointmentHandler handler,
        ICompanyHandler companyHandler,
        [FromQuery]Guid id,
        [FromQuery]DateTime date)
    {
        Company? company = null;
        try
        {
            company = await companyHandler.GetByCodeAsync(id);
            if(company is null)
                return TypedResults.NotFound(new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(null,
                    404, "Empresa não encontrada!"));
        }
        catch
        {
            return TypedResults.NotFound(new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(null,
                404, "Não foi possível validar a empresa!"));
        }
        
        var request = new GetEmployeesWithOpenScheduleRequest()
        {
            Date = date,
            UserId = company.UserId
        };
        
        var result = await handler.GetEmployeesWithOpenScheduleAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}