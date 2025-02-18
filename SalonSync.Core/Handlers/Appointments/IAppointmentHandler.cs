using SalonSync.Core.DTOs;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Appointments;

public interface IAppointmentHandler
{
    Task<Response<AppointmentCreatedDto?>> CreateAsync(GenerateScheduleWithEmployeeRequest request);
    Task<Response<List<AvailableDateDto>?>> GetAvailableDatesAsync(GetAvailableDateRequest request);
    Task<Response<List<AvailableDateDto>?>> GetAvailableDatesByCompanyCodeAsync(GetAvailableDateByCompanyRequest request);
    Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request);
    Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request);
    Task<Response<SchedullingClosedDTO?>> FinishAsync(FinishSchedulingRequest request);
    Task<Response<SchedullingClosedDTO?>> CancelAsync(FinishSchedulingRequest request);
    Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(GetEmployeesWithOpenScheduleRequest request);
    Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request);
    Task<PagedResponse<List<AppointmentDto>?>> GetByEmployeeCodeAsync(GetAppointmentByEmployeeCodeRequest request);
}