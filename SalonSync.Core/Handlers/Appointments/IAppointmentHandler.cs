using SalonSync.Core.DTOs;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Appointments;

public interface IAppointmentHandler
{
    Task<Response<AppointmentCreatedDto?>> CreateAsync(GenerateScheduleWithEmployeeRequest request);
    Task<Response<List<AvailableDateDto>?>> GetAvailableDatesAsync(GetAvailableDateRequest request);
    Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request);
    Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request);
    Task<Response<SchedulingClosedDTO?>> FinishAsync(FinishSchedulingRequest request);
    Task<Response<SchedulingClosedDTO?>> CancelAsync(FinishSchedulingRequest request);
    Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(GetEmployeesWithOpenScheduleRequest request);
    Task<PagedResponse<List<Appointment>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request);
    Task<PagedResponse<List<Appointment>?>> GetByEmployeeCodeAsync(GetAppointmentByEmployeeCodeRequest request);
}