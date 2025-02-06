using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Data;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Services;

public class ServiceHandler(AppDbContext context) : IServiceHandler
{
    public async Task<Response<Service?>> CreateAsync(CreateServiceRequest request)
    {
        try
        {
            if(await MethodsExtensions.CheckFieldInUse(context.Services, 
                   x => x.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
                        && x.UserId == request.UserId))
                return new Response<Service?>(null, 400, "Já existe um serviço cadastrado com o mesmo nome!");

            Service service = new()
            {
                Name = request.Name,
                Duration = request.Duration,
                Price = request.Price,
                UserId = request.UserId
            };

            await context.Services.AddAsync(service);
            await context.SaveChangesAsync();

            return new Response<Service?>(service, 201, "Serviço cadastrado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Service?>(null, 500, "(SSS001) Não foi possível cadastrar o serviço!");
        }
    }

    public async Task<Response<Service?>> UpdateAsync(UpdateServiceRequest request)
    {
        try
        {
            var service = await context
                .Services
                .FirstOrDefaultAsync(x => x.Id == request.Id
                            && x.UserId == request.UserId);

            if (service is null)
                return new Response<Service?>(null, 404, "Serviço não encontrado!");
            
            if (!service.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase))
            {
                if(await MethodsExtensions.CheckFieldInUse(context.Employees, 
                       x => x.Email.Equals(request.Name, StringComparison.OrdinalIgnoreCase)
                            && x.UserId == request.UserId))
                    return new Response<Service?>(null, 400, "Já existe um serviço cadastrado com o mesmo nome!");
            }

            service.Name = request.Name;
            service.Duration = request.Duration;
            service.Price = request.Price;

            context.Services.Update(service);
            await context.SaveChangesAsync();

            return new Response<Service?>(service, message: "Serviço atualizado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Service?>(null, 500, "(SSS002) Não foi possível alterar o serviço!");
        }
    }

    public async Task<Response<Service?>> DeleteAsync(DeleteServiceRequest request)
    {
        try
        {
            var service = await context
                .Services
                .FirstOrDefaultAsync(x => x.Id == request.Id
                            && x.UserId == request.UserId);
            
            if (service is null)
                return new Response<Service?>(null, 404, "Serviço não encontrado!");

            context.Services.Remove(service);
            await context.SaveChangesAsync();

            return new Response<Service?>(service, message: "Serviço excluído com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Service?>(null, 500, "(SSS003) Não foi possível excluir o serviço!");
        }
    }

    public async Task<Response<Service?>> GetByIdAsync(GetServiceByIdRequest request)
    {
        try
        {
            var service = await context
                .Services
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id
                                          && x.UserId == request.UserId);
            
            return service is null
                ?  new Response<Service?>(null, 404, "Serviço não encontrado!")
                :  new Response<Service?>(service, message: "Serviço encontrado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Service?>(null, 500, "(SSS004) Não foi possível recuperar o serviço!");
        }
    }

    public async Task<PagedResponse<List<Service>?>> GetAllAsync(GetAllServicesRequest request)
    {
        try
        {
            var query = context
                .Services
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            var services = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Service>?>(services, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Service>?>(null, 500, "(SSS005) Não foi possível recuperar os serviços!");
        }
    }
}