using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Appointments;

public class AppointmentHandler(AppDbContext context) : IAppointmentHandler
{
    public async Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request)
    {
        try
        {
            var employee = await context
                .Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.EmployeeId);
            
            if (employee is null)
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 404,
                    "Não foi possível encontrar o profissional!");

            request.StartDate = request.StartDate.Date;
            request.EndDate = request.EndDate.Date;
            
            if(await CheckForScheduleConflictsAsync(request))
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Já existe uma agenda definida para esse profissional para o período especificado!");

            var timeRange = SetTimeRange(employee.StartTime ?? new TimeOnly(8, 0),
                employee?.EndTime ?? new TimeOnly(18, 0));
            if (!timeRange.Any())
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Não foi possível carregar o intervalo de horas!");

            var dateRange = SetDateRange(request.StartDate, request.EndDate);
            if (!dateRange.Any())
                return new Response<EmployeeAvailabilityScheduleDTO?>(null, 400,
                    "Não foi possível definir as datas para agendamento!");
            
            List<Appointment> appointments = dateRange
                .SelectMany(date => timeRange.Select(x => new Appointment
                {
                    UserId = request.UserId,
                    Date = date,
                    StartTime = x.Key,
                    EndTime = x.Value,
                    EmployeeId = request.EmployeeId,
                    Status = EAppointmentStatus.Free,
                })).ToList();

            await context.Appointments.AddRangeAsync(appointments);
            await context.SaveChangesAsync();

            EmployeeAvailabilityScheduleDTO dto = new()
            {
                Id = employee?.Id ?? request.EmployeeId,
                Name = employee?.Name ?? string.Empty,
                AvailableDays = dateRange,
                CountDays = dateRange.Count,
                UserId = request.UserId
            };

            return new Response<EmployeeAvailabilityScheduleDTO?>(dto, 201, "Agenda criada com sucesso!");
        }
        catch (Exception ex)
        {
            return new Response<EmployeeAvailabilityScheduleDTO?>(null, 500, "(SSA001) Não foi abrir a agenda do profissional!");
        }
    }
    
    public async Task<Response<Appointment?>> CreateAsync(GenerateScheduleWithEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    

    public async Task<Response<SchedulingClosedDTO?>> FinishAsync(FinishSchedulingRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(GetEmployeesWithOpenScheduleRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<List<Appointment>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request)
    {
        throw new NotImplementedException();
    }

    private async Task<bool> CheckForScheduleConflictsAsync (OpenAppointmentByEmployeeRequest request)
    {
        try
        {
            var conflict = await context
                .Appointments
                .AsNoTracking()
                .AnyAsync(x => x.UserId == request.UserId
                     && x.EmployeeId == request.EmployeeId
                     && x.Date >= request.StartDate.Date && x.Date <= request.EndDate.Date);

            return conflict;
        }
        catch (Exception)
        {
            return false;
        }
    }

    private Dictionary<TimeOnly, TimeOnly> SetTimeRange(TimeOnly startTime, TimeOnly endTime)
    {
        try
        {
            Dictionary<TimeOnly, TimeOnly> timeRange = [];

            for (TimeOnly hour = startTime; hour < endTime; hour = hour.AddMinutes(ApiConfiguration.DefaultTimeByAppointment))
            {
                TimeOnly endInterval = hour.AddMinutes(ApiConfiguration.DefaultTimeByAppointment);
                
                timeRange.Add(hour, endInterval > endTime ? endTime : endInterval);
            }
            
            return timeRange;
        }
        catch
        {
            return new Dictionary<TimeOnly, TimeOnly>();
        }
    }

    private List<DateTime> SetDateRange(DateTime start, DateTime end)
    {
        try
        {
            List<DateTime> days = days = Enumerable
                .Range(0, (end - start).Days + 1)
                .Select(x => start.AddDays(x).Date)
                .Where(date => date.DayOfWeek != DayOfWeek.Sunday)
                .ToList();

            return days;
        }
        catch
        {
            return new List<DateTime>();
        }
    }
}