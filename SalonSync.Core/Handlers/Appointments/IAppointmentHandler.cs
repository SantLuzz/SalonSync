using SalonSync.Core.DTOs;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Appointments;

public interface IAppointmentHandler
{
    Task<Response<Appointment?>> CreateAsync(GenerateScheduleWithEmployeeRequest request);
    Task<Response<EmployeeAvailabilitySlotsDTO?>> GetAvailablesHoursByEmployeeIdAsync(GetAvailableHoursByServicesAndEmployeeRequest request);
    Task<Response<EmployeeAvailabilityScheduleDTO?>> OpenByEmployeeIdAsync(OpenAppointmentByEmployeeRequest request);
    Task<Response<SchedulingClosedDTO?>> FinishAsync(FinishSchedulingRequest request);
    Task<PagedResponse<List<EmployeeAvailabilitySlotsDTO>?>> GetEmployeesWithOpenScheduleAsync(GetEmployeesWithOpenScheduleRequest request);
    Task<PagedResponse<List<Appointment>?>> GetByEmployeeAsync(GetAppointmentByEmployeeRequest request);
    
}