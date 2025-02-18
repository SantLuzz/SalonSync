using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Entities;

public interface IEmployeeHandler
{
    Task<Response<Employee?>> CreateAsync(CreateEmployeeRequest request);
    Task<Response<Employee?>> UpdateAsync(UpdateEmployeeRequest request);
    Task<Response<Employee?>> UpdateImageAsync(UpdateImageEmployeeRequest request);
    Task<Response<Employee?>> DeleteAsync(DeleteEmployeeRequest request);
    Task<Response<Employee?>> GenerateUniqueCodeAsync(GenerateUniqueEmployeeCodeRequest request);
    Task<Response<Employee?>> GetByIdAsync(GetEmployeeByIdRequest request);
    Task<PagedResponse<List<Employee>?>> GetAllAsync(GetAllEmployeesRequest request);
    
}