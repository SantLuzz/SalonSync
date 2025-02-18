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

public class GetAvailableHoursByEmployeeIdAndCompanyCodeEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/employees-available-hours/", HandlerAsync)
            .WithName("Schedules: Get available hours for employee id")
            .WithDescription("Get available hours for employee id.")
            .WithOrder(5)
            .Produces<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>>();

    private static async Task<IResult> HandlerAsync(
        IAppointmentHandler handler,
        ICompanyHandler companyHandler,
        [FromQuery]Guid id,
        [FromQuery]DateTime date,
        [FromQuery]long employeeId,
        [FromQuery]long serviceId)
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
        
        
        var request = new GetAvailableHoursByServicesAndEmployeeRequest()
        {
            Date = date,
            UserId = company.UserId,
            EmployeeId = employeeId,
            ServiceId = serviceId
        };
        
        var result = await handler.GetAvailablesHoursByEmployeeIdAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
    }
}