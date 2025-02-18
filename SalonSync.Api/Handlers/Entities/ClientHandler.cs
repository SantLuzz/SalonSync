using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Data;
using SalonSync.Api.Handlers.Appointments;
using SalonSync.Api.Services;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Entities;

public class ClientHandler(AppDbContext context) : IClientHandler
{
    private readonly ClientPackageAppointmentHandler _clientAppointmentHandler = new ClientPackageAppointmentHandler(context);
    public async Task<Response<Client?>> CreateAsync(CreateClientRequest request)
    {
        try
        {
            if(await MethodsExtensions.CheckFieldInUse(context.Clients, 
                   x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                return new Response<Client?>(null, 400, "O e-Mail informado já está vinculado a outro cliente!");
            
            var client = new Client
            {
                Name = request.Name,
                Email = request.Email,
                Phone = request.Phone,
                PreferredService = request.PreferredService,
                Observations = request.Observations,
                Type = request.Type,
                CreatedAt = DateTime.Now,
                UserId = request.UserId,
                
            };
            
            await context.Clients.AddAsync(client);
            await context.SaveChangesAsync();

            return new Response<Client?>(client, 201, "Cliente criado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL001) Não foi possível cadastrar o cliente!");
        }
    }

    public async Task<Response<Client?>> UpdateAsync(UpdateClientRequest request)
    {
        try
        {
            var client = await context
                .Clients
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                    && x.Id == request.Id);

            if (client is null)
                return new Response<Client?>(null, 404, "Cliente não encontrado!");

            if (!client.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase))
            {
                if(await MethodsExtensions.CheckFieldInUse(context.Clients, 
                       x => x.Email.Equals(request.Email, StringComparison.OrdinalIgnoreCase)))
                    return new Response<Client?>(null, 400, "O e-Mail informado já está vinculado a outro cliente!");
            }
            
            client.Name = request.Name;
            client.Email = request.Email;
            client.Phone = request.Phone;
            client.PreferredService = request.PreferredService;
            client.Observations = request.Observations;
            client.Type = request.Type;
            client.UserId = request.UserId;
            client.UpdatedAt = DateTime.Now;

            context.Clients.Update(client);
            await context.SaveChangesAsync();

            return new Response<Client?>(client, message: "Cliente alterado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL002) Não foi possível alterar o cliente!");
        }
    }

    public async Task<Response<Client?>> UpdateImageAsync(UpdateImageClientRequest request)
    {
        try
        {
            var client = await context
                .Clients
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);
            
            if (client is null)
                return new Response<Client?>(null, 404, "Cliente não encontrado!");
            
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            
            var data = new Regex(@"^data:image/[a-z]+;base64,")
                .Replace(request.Base64Image, string.Empty);
            
            var bytes = Convert.FromBase64String(data);

            try
            {
                await File.WriteAllBytesAsync($"{ApiConfiguration.FileFolder}{fileName}", bytes);
            }
            catch (Exception)
            {
                return new Response<Client?>(null, 500, "(SSCL004) Não foi possível gravar a imagem do cliente!");
            }

            client.Image = ApiConfiguration.UrlFiles + fileName;
            client.UpdatedAt = DateTime.Now;
            
            context.Clients.Update(client);
            await context.SaveChangesAsync();
            
            return new Response<Client?>(client, message: "Imagem alterada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL003) Não foi possível alterar a imagem do cliente!");
        }
    }

    public async Task<Response<Client?>> UpdateLastedProcedureAsync(UpdateLastedProcedureClientRequest request)
    {
        try
        {
            var client = await context
                .Clients
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);
            if (client is null)
                return new Response<Client?>(null, 404, "Cliente não encontrado!");
            
            client.LastProcedure = request.LastProcedure;
            client.UpdatedAt = DateTime.Now;
            
            context.Clients.Update(client);
            await context.SaveChangesAsync();
            
            return new Response<Client?>(client, message: "Última visita alterada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL005) Não foi possível alterar a data da última visita do cliente!");
        }
    }

    public async Task<Response<Client?>> DeleteAsync(DeleteClientRequest request)
    {
        try
        {
            var client = await context
                .Clients
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);
            if (client is null)
                return new Response<Client?>(null, 404, "Cliente não encontrado!");

            context.Clients.Remove(client);
            await context.SaveChangesAsync();
            
            return new Response<Client?>(client, message: "cliente excluído com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL006) Não foi possível remover o cliente!");
        }
    }

    public async Task<Response<Client?>> GetByIdAsync(GetClientByIdRequest request)
    {
        try
        {
            var client = await context
                .Clients
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId
                                          && x.Id == request.Id);

            if (client is null)
                return new Response<Client?>(null, 404, "Cliente não encontrado!");

            if (client is { Type: EClientType.Monthly })
            {
                client.ClientPackages = await context
                    .ClientPackages
                    .AsNoTracking()
                    .Include(x => x.Package)
                    .Where(x => x.ClientId == client.Id
                        && x.UserId == request.UserId
                        && x.Status == EPackageStatus.Active)
                    .ToListAsync();

                client.Historic = await _clientAppointmentHandler
                    .GetHistoricClient(client.Id, request.UserId);
            }
            
            return new Response<Client?>(client, message: "Cliente encontrado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Client?>(null, 500, "(SSCL007) Não foi possível buscar o cliente!");
        }
    }

    public async Task<PagedResponse<List<ClientBirthdaysDTO>?>> GetBirthdaysOfWeek(GetBirthdaysOfWeekRequest request)
    {
        DateService dateService;
        try
        {
            dateService = new DateService();
            dateService.GetCurrentWeekRange();
        }
        catch (Exception)
        {
            return new PagedResponse<List<ClientBirthdaysDTO>?>(null, 500, "(SSCL011) Não foi possível converter as datas!");
        }
        
        
        try
        {
            var clients = await context
                .Clients
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId
                && x.Birthdate >= dateService.StartOfWeek && x.Birthdate <= dateService.EndOfWeek)
                .ToListAsync();

            return new PagedResponse<List<ClientBirthdaysDTO>?>(clients
                    .Select(x => new ClientBirthdaysDTO
                {
                    Id = x.Id,
                    Birthday = x.Birthdate,
                    Name = x.Name ?? string.Empty,
                }).ToList(), 
                message:"Aniversáriantes encontrados com sucesso!");
        }
        catch (Exception)
        {
            return new PagedResponse<List<ClientBirthdaysDTO>?>(null, 500, "(SSCL010) Não foi possível buscar os aniversáriantes da semana!");
        }
    }

    public async Task<PagedResponse<List<Client>?>> GetAllAsync(GetAllClientsRequest request)
    {
        try
        {
            var query = context
                .Clients
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            var clients = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Client>?>(clients, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Client>?>(null, 500, "(SSCL008) Não foi possível buscar os clientes!");
        }
    }

    public async Task<PagedResponse<List<Client>?>> GetAllByTypeAsync(GetAllClientsByTypeRequest request)
    {
        try
        {
            var query = context
                .Clients
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId
                    && x.Type == request.Type);

            var clients = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Client>?>(clients, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Client>?>(null, 500, "(SSCL009) Não foi possível buscar os clientes pelo tipo!");
        }
    }
    
}