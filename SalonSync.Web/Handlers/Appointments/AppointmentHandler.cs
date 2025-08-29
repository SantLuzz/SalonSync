using System.Net.Http.Json;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Web.Handlers.Appointments;

public class AppointmentHandler(IHttpClientFactory httpClientFactory) : IAppointmentHandler
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient(WebConfiguration.HttpClientName);
    private const string Format = "yyyy-MM-dd";
    public async Task<Response<AppointmentCreatedDto?>> CreateAsync(GenerateScheduleWithEmployeeRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/appointments", request);
        return await result.Content.ReadFromJsonAsync<Response<AppointmentCreatedDto?>>()
               ?? new Response<AppointmentCreatedDto?>(null, 400, "Não foi possível criar o agendamento!");
    }

    public async Task<Response<List<AppointmentCreatedDto>?>> CreateAsync(GenerateAutoScheduleRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync("v1/appointments/auto-schedule", request);
        return await result.Content.ReadFromJsonAsync<Response<List<AppointmentCreatedDto>?>>()
               ?? new Response<List<AppointmentCreatedDto>?>(null, 400, "Não foi possível criar os agendamento!");
    }

    public async Task<Response<List<AvailableDateDto>?>> GetAvailableDatesAsync(GetAvailableDateRequest request)
        => await _httpClient.GetFromJsonAsync<Response<List<AvailableDateDto>?>>("v1/appointments/available-date/")
        ?? new Response<List<AvailableDateDto>?>(null, 404, "Nenhum horário disponível!");
    
    public async Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request)
    {
        var date =  request.Date != DateTime.MinValue || request.Date != DateTime.MaxValue
            ? request.Date.ToString(Format) 
            : DateTime.Now.GetFirstDay().ToString(Format);
        
        var url = $"v1/appointments/employees-available-hours?date={date}&employeeId={request.EmployeeId}&serviceId={request.ServiceId}";
        return await _httpClient.GetFromJsonAsync<Response<EmployeeAvailabilitySlotsDTO?>>(url)
               ?? new Response<EmployeeAvailabilitySlotsDTO?>(null, 404,
                   "Não foi possível encontrar os horários livre do profissíonal!");
    }

    public async Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request)
    {
        var result = await _httpClient.PostAsJsonAsync($"v1/appointments/{request.EmployeeId}/open", request);
        return await result.Content.ReadFromJsonAsync<Response<EmployeeAvailabilityScheduleDTO?>>()
            ?? new Response<EmployeeAvailabilityScheduleDTO?>(null, 400, "Não foi possível abrir a agenda do profissíonal!");
    }

    public async Task<Response<SchedullingClosedDTO?>> FinishAsync(FinishSchedulingRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync("v1/appointments/finish", request);
        return await result.Content.ReadFromJsonAsync<Response<SchedullingClosedDTO?>>()
               ?? new Response<SchedullingClosedDTO?>(null, 400, "Não foi possível encerrar o agendamento!");
    }

    public async Task<Response<SchedullingClosedDTO?>> CancelAsync(FinishSchedulingRequest request)
    {
        var result = await _httpClient.PutAsJsonAsync("v1/appointments/cancel", request);
        return await result.Content.ReadFromJsonAsync<Response<SchedullingClosedDTO?>>()
               ?? new Response<SchedullingClosedDTO?>(null, 400, "Não foi possível cancelar o agendamento!");
    }

    public async Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(
        GetEmployeesWithOpenScheduleRequest request)
        => await _httpClient.GetFromJsonAsync<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>>("v1/appointments/employees-open-schedule/")
           ?? new PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>(null, 404, "Não foi possível recuperar os profissíonais com agenda aberta");

    public async Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request)
    {
        var date =  request.Date != DateTime.MinValue || request.Date != DateTime.MaxValue
            ? request.Date.ToString(Format) 
            : DateTime.Now.GetFirstDay().ToString(Format);

        var url = $"v1/appointments/employees-appointments/{request.EmployeeId}?date={date}";
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<AppointmentDto>?>>(url)
            ?? new PagedResponse<List<AppointmentDto>?>(null, 404,"Não foi possível recuperar os agendamentos do profissíonal!");
    }

    public async Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeCodeAsync(GetAppointmentByEmployeeCodeRequest request)
    {
        var date = request.Date != DateTime.MinValue || request.Date != DateTime.MaxValue
            ? request.Date.ToString(Format)
            : DateTime.Now.GetFirstDay().ToString(Format);

        var url = $"v1/schedules/employees-appointments?id={request.Code}&date={date}";
        return await _httpClient.GetFromJsonAsync<PagedResponse<List<AppointmentDto>?>>(url)
            ?? new PagedResponse<List<AppointmentDto>?>(null, 404, "Não foi possível recuperar os agendamentos do profissíonal!");
    }
    
    public async Task<Response<List<AvailableDateDto>?>> GetAvailableDatesByCompanyCodeAsync(GetAvailableDateByCompanyRequest request)
    {
        throw new NotImplementedException();
    }
}