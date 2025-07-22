using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Web.Components.Dialogs;
using SalonSync.Web.Services;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Appointments;

public partial class ListAppointmentsPage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAppointmentHandler Handler { get; set; } = null!;
    [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
    [Inject] public ControlAppointmentsClientService ClientService { get; set; } = null!;
    [Inject] public PaymentService PaymentService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public bool IsStarted { get; set; }
    public List<AppointmentDto> Appointments { get; set; } = [];
    public Employee Employee { get; set; } = new();
    public List<Employee> Employees { get; set; } = [];
    public DateTime? Date { get; set; }
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsStarted = true;
        Date = DateTime.Today;
        await GetEmployeesAsync();
        await GetAppointmentsAsync();
        IsStarted = false;
    }

    #endregion

    #region Methods

    public async void OnSearchAsync()
    {
        await GetAppointmentsAsync();
        StateHasChanged();
    }

    protected async void CreateNewAppointment()
    {
        try
        {
            if (!Appointments.Any())
            {
                OnSearchAsync();
            }

            bool diferenceSelected =  Appointments.Any(x => x.EmployeeId != Employee.Id);

            if (diferenceSelected)
            {
                Snackbar.Add("O profissional selecionado não teve sua lista carregada, carregue para prosseguir!",
                    Severity.Warning);
                return;
            }

            if (!Appointments.Any())
            {
                Snackbar.Add("O profissional selecionado não teve sua agenda aberta, abra para prosseguir!",
                    Severity.Warning);
                return;
            }
            
            NavigationManager.NavigateTo($"/appointments/create?employeeId={Employee.Id}");
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    public async void OnCloseAppointment(AppointmentDto appointment)
    {
        IsStarted = true;
        try
        {
            var response = await DialogService.ShowMessageBox("Agendamento", $"Deseja finalizar o agendamento {appointment?.Id}?",
                yesText: "SIM", noText: "NÃO");

            if (response is false)
            {
                Snackbar.Add("Operação cancelada pelo operado!", Severity.Warning);
                return;
            }
            
            
            var finish = new FinishSchedulingRequest
            {
                Id = appointment.Id,
                AppointmentGroupId = appointment.AppointmentGroupId,
                Date = appointment.Date,
                EmployeeId = appointment.EmployeeId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = EAppointmentStatus.Completed,
                ServiceId = appointment.ServiceId ?? 0
            };
            
            var result = await Handler.FinishAsync(finish);
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível finalizar o agendamento!", Severity.Error);
                return;                
            }
            
            Snackbar.Add("Agendamento finalizado com sucesso!", Severity.Success);
            
            var scheuduling = result.Data.Appointments
                .FirstOrDefault(x => x.ClientPackageAppointments is not null);
            if (scheuduling is not null)
            {
                //tem pacote devo informar a sessão e não efetua o pagamento
                await ClientService.ReportSessionUsedAndLastProcedure(scheuduling);
            }
            else
            {
                var reported = await ClientService.ReportLastProcedure(appointment.ClientId ?? 0);
                
                response = await DialogService.ShowMessageBox("Pagamento", "Deseja efetuar o lançamento no caixa?",
                    yesText: "SIM", noText: "NÃO");
            
                if (response is true)
                {
                    var payment = await PaymentService.GetPaymentDetailsAndPay(new PaymentDetailsViewModel
                    {
                        Amount = appointment.Service?.Price ?? 0,
                        Description = CreateHistoricPayment(appointment),
                        EntityType = EEntityType.Appointments,
                        EntityId = appointment.Id,
                        PaidOrReceivedAt = DateTime.Now,
                        CenterType = ECostCenterType.Income
                    });
                }
            }

            await GetAppointmentsAsync();
            IsStarted = false;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
            IsStarted = false; 
        }
        finally { IsStarted = false; }
    }
    public async void OnDeleteButtonClickedAsync(AppointmentDto appointment)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o agendamento {appointment.Id} será cancelado. " +
                                       $"Esta é uma ação irreversível, deseja prosseguir?", "Cancelar Agendamento", cancelText: "Não Cancelar");

        if (result is true)
            await OnDeleteAsync(appointment);

        StateHasChanged();
    }

    private async Task OnDeleteAsync(AppointmentDto appointment)
    {
        IsStarted = true;
        try
        {
            var parameters = new DialogParameters
            {
                { "FieldLabel", "Digite sua mensagem" },
                { "Value", CreateReasonCancellation(appointment) },
                { "MinLength", 5 }, 
                { "MaxLength", 255 }
            };
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                Position = DialogPosition.Center
            };
            
            var dialog =  await DialogService
                .ShowAsync<TextFieldDialog>("Informe a Justificativa de Cancelamento", parameters, options);
            
            var dialogResult = await dialog.Result;
            if (dialogResult is { Canceled: true })
            {
                Snackbar.Add("Justificativa não informada, operação cancelada!", Severity.Warning);
                return;
            }
            
            var cancel = new FinishSchedulingRequest
            {
                Id = appointment.Id,
                AppointmentGroupId = appointment.AppointmentGroupId,
                CancellationReason = (string)(dialogResult?.Data ?? string.Empty),
                Date = appointment.Date,
                EmployeeId = appointment.EmployeeId,
                StartTime = appointment.StartTime,
                EndTime = appointment.EndTime,
                Status = EAppointmentStatus.Cancelled,
                ServiceId = appointment.ServiceId ?? 0
            };
            
            var result = await Handler.CancelAsync(cancel);

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    Appointments.RemoveAll(x => x.Id == appointment.Id);
                    Snackbar.Add($"Agendamento {appointment.Id} cancelado com sucesso!", Severity.Success);
                }
                else
                {
                    Snackbar.Add(result.Message, Severity.Warning);
                }
            }
            else
                Snackbar.Add(result.Message, Severity.Success);
            
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }

    private async Task GetEmployeesAsync()
    {
        try
        {
            var result = await EmployeeHandler.GetAllAsync(new GetAllEmployeesRequest()
            {
                PageNumber = 1,
                PageSize = 1000,
                
            });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar os profissíonais cadastrados!", Severity.Warning);
                NavigationManager.NavigateTo("/employees");
            }

            Employees = result.Data ?? [];
            Employee = Employees.FirstOrDefault() ?? new Employee();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    
    private async Task GetAppointmentsAsync()
    {
        try
        {
            var request = new GetAppointmentByEmployeeRequest()
            {
                EmployeeId = Employee.Id,
                Date = Date ?? DateTime.Today
            };
            
            var result = await Handler.GetByEmployeeAsync(request);

            if (result is { IsSuccess: true })
                Appointments = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private string CreateReasonCancellation(AppointmentDto appointment)
        =>  $"O agendamento #{appointment.Id.ToString().PadLeft(6, '0')} do profissional {appointment.Employee?.Name ?? "Indefinido"}, " +
            $"marcado para {appointment.Date:dd/MM/yyyy} das {appointment.StartTime} às {appointment.EndTime}, " +
            $"foi cancelado pelo seguinte motivo: ";

    private string CreateHistoricPayment(AppointmentDto appointment)
        => $"Pagamento referente ao agendamento #{appointment.Id.ToString().PadLeft(6, '0')}" +
           $" {(appointment.Client is not null ? $"para o cliente ({appointment.Client.Id}) {appointment.Client.Name}" : "")}" +
           $" {(appointment.Employee is not null ? $"realizado pelo profissional ({appointment.Employee.Id}) {appointment.Employee.Name}" : "")}";

    #endregion
}