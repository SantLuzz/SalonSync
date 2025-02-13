using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Entities;

public interface ICompanyHandler
{
    Task<Response<Company?>> CreateAsync(CreateCompanyRequest request);
    Task<Response<Company?>> UpdateAsync(UpdateCompanyRequest request);
    Task<Response<Company?>> DeleteAsync(DeleteCompanyRequest request);
    Task<Response<Company?>> GetAsync(GetCompanyRequest request);
    Task<Response<Company?>> GenerateCodeAsync(GenerateCodeByCompanyRequest request);
}