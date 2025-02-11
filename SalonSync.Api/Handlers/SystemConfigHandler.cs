using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Handlers;
using SalonSync.Core.Models;
using SalonSync.Core.Requests.Configs;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers;

public class SystemConfigHandler(AppDbContext context) : ISystemConfigHandler
{
    
    public async Task<Response<SystemConfig?>> UpdateAsync(UpdateSystemConfigRequest request)
    {
        try
        {
            var config = await context
                .SystemConfigs
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            config = config ?? new SystemConfig()
            {
                UserId = request.UserId,
            };
            
            config.Monday = request.Monday;
            config.Tuesday = request.Tuesday;
            config.Wednesday = request.Wednesday;
            config.Thursday = request.Thursday;
            config.Friday = request.Friday;
            config.Saturday = request.Saturday;
            config.Sunday = request.Sunday;
            
            if(config.Id == 0)
                await context.SystemConfigs.AddAsync(config);
            else
                context.SystemConfigs.Update(config);
            
            await context.SaveChangesAsync();
            
            return new Response<SystemConfig?>(config, message: "Configurações alteradas com sucesso!");
        }
        catch (Exception ex)
        {
            return new Response<SystemConfig?>(null, 500, "(SSSC001) Não foi possível alterar as configurações do app!");
        }
    }

    public async Task<Response<SystemConfig?>> GetConfigAsync(GetSystemConfigRequest request)
    {
        try
        {
            var config = await context
                .SystemConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            return config is null
                ?  new Response<SystemConfig?>(null, 404, "Configurações não encontradas!")
                :  new Response<SystemConfig?>(config, message: "Configurações encontradas com sucesso!");
        }
        catch (Exception ex)
        {
            return new Response<SystemConfig?>(null, 500, "(SSSC002) Não foi possível recuperar as configurações do app!");
        }
    }
    
    public async Task<List<DayOfWeek>> GetDaysOfWeekAsync(string userId)
    {
        try
        {
            var config = await context
                .SystemConfigs
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            List<DayOfWeek> days = config is not null
                ? typeof(SystemConfig).GetProperties()
                    .Where(p => p.PropertyType == typeof(bool) && (bool)p.GetValue(config)!)
                    .Select(p => Enum.Parse<DayOfWeek>(p.Name))
                    .ToList()
                : [];

            return days;
        }
        catch (Exception ex)
        {
            return [];
        }
    }
}