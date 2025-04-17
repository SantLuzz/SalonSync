using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Requests.Services;
using SalonSync.Web.Components.Dialogs;

namespace SalonSync.Web.Pages.Appointments;

public partial class CreateAppointmentsPage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
    [Inject] public IClientHandler ClientHandler { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    [Inject] public IServiceHandler ServiceHandler { get; set; } = null!;
    [Inject] public IAppointmentHandler AppointmentHandler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    
    #endregion
    
    #region Properties
    [Parameter]
    [SupplyParameterFromQuery]
    public long? EmployeeId { get; set; }
    public Employee Employee { get; set; } = new();
    public List<Client> Clients { get; set; } = [];
    public Client SelectedClient { get; set; } = new();
    public List<Service> Services { get; set; } = [];
    public List<AvailableDateDto> AvailableDateDtos { get; set; } = [];
    public EmployeeAvailabilitySlotsDTO AvailabilitySlotsDto { get; set; } = new();
    public bool IsStarted { get; set; }
    public bool IsSubmit { get; set; }
    public bool DateIsSelected { get; set; }
    public GenerateScheduleWithEmployeeRequest InputModel { get; set; } = new();
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsStarted = true;
        await GetEmployee();
        await GetAvailableDate();
        await GetClients();
        IsStarted = false;
    }
    
    #endregion

    #region Protected Methods
    protected async Task OnAddClient()
    {
        try
        {
            var parameters = new DialogParameters
            {
                { "ClientId", InputModel.ClientId }
            };
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                Position = DialogPosition.Center
            };

            var dialog = await DialogService
                .ShowAsync<CreateClientDialog>("Novo Cliente", parameters, options);

            var result = await dialog.Result;
            if (result is { Canceled: true })
            {
                Snackbar.Add("Operação cancelada!", Severity.Warning);
                return;
            }

            IsStarted = true;
            await GetClients();
            long clientId = (long)(result?.Data ?? 0);
            if (clientId > 0)
            {
                SelectedClient = Clients.FirstOrDefault(x => x.Id == clientId) ?? new();
                await OnChangedClient(SelectedClient);
            }
                
            StateHasChanged();
            IsStarted = false;
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao adicionar o cliente: " + e.Message, Severity.Error);
        }
    }
    
    protected async Task OnChangedClient(Client selectedClient)
    {
        SelectedClient = selectedClient;
        DateIsSelected = false;
        InputModel.ClientId = selectedClient.Id;
        await GetServices();
    }
    
    protected async Task OnChangedService(long id)
    {
        InputModel.ServiceId = id;
        if (DateIsSelected)
        {
            await GetAvailableHours();
            DateIsSelected = true;
            StateHasChanged();
        }
    }
    
    protected async Task SelectedDate(AvailableDateDto availableDate)
    {
        InputModel.Date = availableDate.Date.Date;
        DateIsSelected = false;
        await GetAvailableHours();
        DateIsSelected = true;
        StateHasChanged();
    }

    protected void SelectedTime(Hours time)
    {
        InputModel.StartTime = time.StartTime;
        InputModel.EndTime = time.EndTime;
    }
    
    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            InputModel.EmployeeId = EmployeeId ?? 0;
            
            if(!ValidatedFields()) return;
            
            var result = await AppointmentHandler.CreateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Agendamento criado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/appointments");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o agendamento!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    #endregion

    #region Private Methods

    private bool ValidatedFields()
    {
        if (InputModel.EmployeeId == 0)
        {
            Snackbar.Add("Não foi possível carregar o profissional, não é possível prosseguir!", Severity.Warning);
            return false;
        }
        
        if (InputModel.ClientId == 0)
        {
            Snackbar.Add("Informe o cliente para prosseguir!", Severity.Warning);
            return false;
        }
        
        if (InputModel.ServiceId == 0)
        {
            Snackbar.Add("Informe o serviço para prosseguir!", Severity.Warning);
            return false;
        }

        if (InputModel.Date == DateTime.MinValue
            || InputModel.Date == DateTime.MaxValue
            || InputModel.Date < DateTime.Today)
        {
            Snackbar.Add("Selecione uma data valida para prosseguir!", Severity.Warning);
            return false;
        }

        if (InputModel.StartTime == TimeOnly.MinValue
            || InputModel.StartTime == TimeOnly.MaxValue)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }

        if (InputModel.EndTime == TimeOnly.MinValue
            || InputModel.EndTime == TimeOnly.MaxValue)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }
        
        if (InputModel.StartTime > InputModel.EndTime)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }

        return true;
    }
        
    
    private async Task GetEmployee()
    {
        try
        {
            if (!EmployeeId.HasValue)
            {
                Snackbar.Add("Profissional não informado!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }
            
            var result = await EmployeeHandler.GetByIdAsync(new GetEmployeeByIdRequest
            {
                Id = EmployeeId ?? 0
            });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar o profissional!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }

            Employee = result.Data ?? new();
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao buscar o profissional: " + e.Message, Severity.Error);
        }
    }

    private async Task GetClients()
    {
        try
        {
            var result = await ClientHandler.GetAllAsync(new());
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar os clientes!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }
            
            Clients = result.Data ?? new();
            SelectedClient = Clients.FirstOrDefault() ?? new();
            await OnChangedClient(SelectedClient);
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao buscar os clientes: " + e.Message, Severity.Error);
        }
    }

    private async Task GetServices()
    {
        bool startLoad = true;
        if (!IsStarted)
        {
            IsStarted = true;
            startLoad = false;
        }
        
        try
        {
            if (SelectedClient.Type == EClientType.Common)
                await GetAllServices();
            else
                await GetServicesByClientPackage();
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao buscar os serviços: " + e.Message, Severity.Error);
        }
        finally
        {
            if (!startLoad)
                IsStarted = false;
        }
    }

    private async Task GetServicesByClientPackage()
    {
        try
        {
            InputModel.ClientUsedPackage = true;
            var packageClient = await ClientPackageHandler.GetByClientIdAsync(new GetActiveClientPackageRequest
            {
                ClientId = SelectedClient.Id,
            });

            if (packageClient.Data is null)
            {
                Snackbar.Add($"O cliente {SelectedClient.Name} não possuí pacotes ativos no momento. " +
                             $"Ao prosseguir, agendamento será realizado como um cliente comum, verifique!",
                    Severity.Warning);
                await GetAllServices();
                return;
            }

            var result = await ServiceHandler.GetByPackageIdAsync(new GetServicesByPackageIdRequest
            {
                PackageId = packageClient.Data.PackageId,
            });
            
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar os serviços!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }

            if ((result.Data is null || !result.Data.Any()))
            {
                var response = await DialogService.ShowMessageBox("Pacotes", $"Não existem serivços vinculados a esse pacote " +
                     $"{packageClient.Data?.Package?.Name}, vincule para prosseguir ou selecione um novo cliente!",
                    yesText: "VINCULAR", noText: "SELECIONAR OUTRO");
                
                if(response is false) return;

                Snackbar.Add($"Selecione o pacote {packageClient.Data?.Package?.Name} e atribua os seus serviços, redirecionando...", Severity.Warning);
                NavigationManager.NavigateTo("/packages");
                return;
            }

            Services = result.Data ?? [];
            InputModel.ServiceId = Services.FirstOrDefault()?.Id ?? 0;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    private async Task GetAllServices()
    {
        try
        {
            InputModel.ClientUsedPackage = false;
            var result = await ServiceHandler.GetAllAsync(new GetAllServicesRequest());
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar os serviços!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }

            Services = result.Data ?? [];
            InputModel.ServiceId = Services.FirstOrDefault()?.Id ?? 0;
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao buscar os serviços: " + e.Message, Severity.Error);
        }
    }

    private async Task GetAvailableDate()
    {
        try
        {
            var result = await AppointmentHandler.GetAvailableDatesAsync(new GetAvailableDateRequest
            {
                EmployeeId = this.EmployeeId,
            });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar as datas disponíveis do profíssional!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                return;
            }

            if (result.Data is null || !result.Data.Any())
            {
                Snackbar.Add("O profíssional não possuí datas disponíveis, abra a agenda para prosseguir!");
                return;
            }
            
            AvailableDateDtos = result.Data ?? [];
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao obter as datas disponíveis do profissional: " + e.Message, Severity.Error);
        }
    }

    private async Task GetAvailableHours()
    {
        try
        {
            var result = await AppointmentHandler.GetAvailablesHoursByEmployeeIdAsync(
                new GetAvailableHoursByServicesAndEmployeeRequest
                {
                    Date = InputModel.Date,
                    EmployeeId = this.EmployeeId ?? 0,
                    ServiceId = InputModel.ServiceId
                });

            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message, Severity.Warning);
                return;
            }

            AvailabilitySlotsDto = result.Data ?? new();
            AvailabilitySlotsDto.GetHours();
        }
        catch (Exception e)
        {
            Snackbar.Add("Erro ao obter os horários disponíveis do profissional: " + e.Message, Severity.Error);
        }
    }

    #endregion
    
}