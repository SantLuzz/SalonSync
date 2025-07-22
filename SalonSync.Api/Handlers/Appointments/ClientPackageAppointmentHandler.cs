using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Enums;
using SalonSync.Core.Models.Appointments;
using SalonSync.Core.Models.Packages;

namespace SalonSync.Api.Handlers.Appointments;

public class ClientPackageAppointmentHandler(AppDbContext context)
{
    public async Task<ClientPackageAppointment?> CreateAsync(long clientPackageId, long appointmentId, string userId)
    {
        try
        {
            var package = new ClientPackageAppointment
            {
                ClientPackageId = clientPackageId,
                AppointmentId = appointmentId,
                UserId = userId,
            };
            
            await context.ClientPackageAppointments.AddAsync(package);
            await context.SaveChangesAsync();

            return package;
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
                // .Include(x => x.Appointment)
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

    public async Task<ClientPackage?> GetActivePackageByClient(long clientId, string userId)
    {
        try
        {
            var clientPackage = await context
                .ClientPackages
                .AsNoTracking()
                .Include(x => x.Package)
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                                          && x.UserId == userId
                                          && x.Status == EPackageStatus.Active
                                          && x.StartDate.Date <= DateTime.Now.Date
                                          && x.EndDate.Date >= DateTime.Now.Date);
            
            return clientPackage;
        }
        catch (Exception e)
        {
            return null;
        }
    }

    public async Task<ClientPackageAppointment?> GetByAppointmentId(long appointmentId, string userId)
    {
        try
        {
            var appointment = await context
                .ClientPackageAppointments
                .AsNoTracking()
                .Include(x => x.ClientPackages)
                //.Include(x => x.Appointment)
                .FirstOrDefaultAsync(x => x.AppointmentId == appointmentId
                                          && x.UserId == userId);

            return appointment;
        }
        catch (Exception e)
        {
            return null;
        }
    }
    
    public async Task<bool> CancelReportSessionUsedAsync(long clientId, long packageId, string userId)
    {
        try
        {
            var clientPackage = await context
                .ClientPackages
                .FirstOrDefaultAsync(x => x.ClientId == clientId
                                          && x.PackageId == packageId
                                          && x.Status == EPackageStatus.Active
                                          && x.UserId == userId);

            if (clientPackage is null)
                return true;
            
            clientPackage.RemainingSessions++;

            context.ClientPackages.Update(clientPackage);
            await context.SaveChangesAsync();

            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }
}