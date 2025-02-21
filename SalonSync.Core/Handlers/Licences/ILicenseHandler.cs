using SalonSync.Core.DTOs;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Licenses;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers.Licences;

public interface ILicenseHandler
{
    Task<bool> CheckAsync(string userId);
    Task<PagedResponse<List<LicenseDto>?>> GetAllAsync();
    Task<Response<LicenseDto?>> UpdateAsync(UpdateLicenseRequest request);
}