using SalonSync.Core.Models;
using SalonSync.Core.Requests.Configs;
using SalonSync.Core.Responses;

namespace SalonSync.Core.Handlers;

public interface ISystemConfigHandler
{
    Task<Response<SystemConfig?>> UpdateAsync(UpdateSystemConfigRequest request);
    Task<Response<SystemConfig?>> GetConfigAsync(GetSystemConfigRequest request);
}