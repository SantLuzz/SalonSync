using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Web.Components.Dialogs;

namespace SalonSync.Web.Services;

public class ControlAppointmentsClientService(
    IClientHandler clientHandler,
    IClientPackageHandler clientPackageHandler,
    ISnackbar snackbar,
    IDialogService dialogService)
{
    public async Task<bool> ReportSessionUsedAndLastProcedure(AppointmentDto appointment)
    {
        try
        {
            var client = await clientHandler.UpdateLastedProcedureAsync(new UpdateLastedProcedureClientRequest
            {
                Id = appointment.ClientId ?? 0,
                LastProcedure = DateTime.Now,
            });

            if (!client.IsSuccess)
            {
                snackbar.Add(client.Message, Severity.Error);
                return false;
            }

            if (client.Data?.Type == EClientType.Common)
            {
                snackbar.Add("O cliente é do tipo comum e não possuí pacotes ativos!", Severity.Success);
                return false;
            }
                
            
            if (appointment.ClientPackageAppointments is not null)
            {
                var clientPackage = await clientPackageHandler.GetByIdAsync(new GetClientPackageByIdRequest
                {
                    Id = appointment.ClientPackageAppointments.ClientPackageId,
                });

                if (clientPackage.Data is not null)
                {
                    var session = await clientPackageHandler.ReportSessionUsedAsync(new ReportSessionUsedByClientRequest
                    {
                        ClientId = appointment.ClientId ?? 0,
                        Date = appointment.Date,
                        PackageId = clientPackage.Data.PackageId,
                        SessionsUsed = 1,
                    });
                    
                    if(session.IsSuccess)
                        snackbar.Add($"Sessão do cliente informada com sucesso!", Severity.Success);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi possível reportar o uso do pacote: {e.Message}");
            return false;
        }
    }

    public async Task<bool> ReportLastProcedure(long clientId)
    {
        try
        {
            var client = await clientHandler.UpdateLastedProcedureAsync(new UpdateLastedProcedureClientRequest
            {
                Id = clientId,
                LastProcedure = DateTime.Now,
            });

            if (!client.IsSuccess)
            {
                snackbar.Add(client.Message, Severity.Error);
                return false;
            }

            return true;
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi possível reportar o último procedimento do cliente: {e.Message}");
            return false;
        }
    }

    public async Task<bool> AutoScheduleClientPackage(ClientPackage clientPackage)
    {
        try
        {
            var parameters = new DialogParameters
            {
                { "ClientPackage", clientPackage },
            };
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Medium,
                FullWidth = true,
                Position = DialogPosition.Center
            };

            var dialog = await dialogService
                .ShowAsync<AutoScheduleDialog>("Agendamento - Pacotes", parameters, options);

            var result = await dialog.Result;
            if (result is { Canceled: true })
            {
                snackbar.Add("Agendamento não confirmado, operação cancelada!", Severity.Warning);
                return false;
            }

            if (result?.Data is false)
            {
                snackbar.Add("Agendamento não efetuado para o pacote!", Severity.Warning);
                return false;
            }
            
            snackbar.Add("Agendamento para pacote confirmado com sucesso!", Severity.Success);
            return true;
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi realizar o agendamento automático do cliente: {e.Message}");
            return false;
        }
    }
}