using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Models.Appointments;

namespace SalonSync.Api.Handlers.Appointments;

public class ClientPackageAppointmentHandler(AppDbContext context)
{
    public async Task<ClientPackageAppointment?> CreateAsync(ClientPackageAppointment client)
    {
        try
        {
            await context.ClientPackageAppointments.AddAsync(client);
            await context.SaveChangesAsync();

            return client;
        }
        catch (Exception)
        {
            return null;
        }
    }

    public async Task<List<ClientPackageAppointment>?> GetHistoricClient(long idClient, string userId)
    {
        try
        {
            var clientPackageAppointments = await context
                .ClientPackageAppointments
                .AsNoTracking()
                .Include(x => x.ClientPackages)
                .Include(x => x.Appointment)
                .Where(x => x.UserId == userId
                            && x.ClientPackages != null
                            && x.ClientPackages.ClientId == idClient)
                .ToListAsync();

            return clientPackageAppointments;
        }
        catch (Exception)
        {
            return new List<ClientPackageAppointment>();
        }
    }
}