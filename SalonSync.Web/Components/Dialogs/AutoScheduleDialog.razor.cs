using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Core.Requests.Packages;
using SalonSync.Core.Requests.Services;

namespace SalonSync.Web.Components.Dialogs;

public partial class AutoScheduleDialogCode : ComponentBase
{
    #region Parameters
    [Parameter] public ClientPackage ClientPackage { get; set; } = new();
    #endregion
    
    #region Dependencies
    [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
    [Inject] public IPackageHandler PackageHandler { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    [Inject] public IServiceHandler ServiceHandler { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAppointmentHandler AppointmentHandler { get; set; } = null!;
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = null!;
    #endregion

    #region Properties
    protected List<AvailableDateDto> AvailableDateDtos { get; set; } = [];
    protected EmployeeAvailabilitySlotsDTO AvailabilitySlotsDto { get; set; } = new();
    protected List<Employee> Employees { get; set; } = [];
    protected List<Service> Services { get; set; } = [];
    protected Service SelectedService { get; set; } = new();
    protected Employee SelectedEmployee { get; set; } = new();
    protected Package Package { get; set; } = new();
    protected int MaxSessions { get; set; }
    protected List<GenerateScheduleWithEmployeeRequest> CreateSchedules { get; set; } = [];
    protected GenerateScheduleWithEmployeeRequest Create { get; set; } = new();
    protected bool DateIsSelected { get; set; }
    protected bool IsBusy { get; set; }
    protected bool IsAddschedule { get; set; }
    protected bool IsFullySchedule { get; set; } = true;
    #endregion
    
    
    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        await GetPackageAsync();
        await GetEmployeesAsync();
        await GetAvailableDate();
        await GetServicesByClientPackage();
        MaxSessions = ClientPackage.Package?.TotalSessions ?? 0;
        Create.ClientId = ClientPackage.ClientId;
        IsBusy = false;
    }
    
    protected async Task SelectedDate(AvailableDateDto availableDate)
    {
        Create.Date = availableDate.Date.Date;
        DateIsSelected = false;
        await GetAvailableHours();
        DateIsSelected = true;
        StateHasChanged();
    }

    protected void SelectedTime(Hours time)
    {
        Create.StartTime = time.StartTime;
        Create.EndTime = time.EndTime;
    }
    
    protected async Task OnChangedService(Service service)
    {
        SelectedService = service;
        Create.ServiceId = service.Id;
        IsServiceFullyScheduled(service.Id);
        if (DateIsSelected)
        {
            await GetAvailableHours();
            DateIsSelected = true;
            StateHasChanged();
        }
    }

    protected async Task OnChangedEmployee(Employee employee)
    {
        SelectedEmployee = employee;
        Create.EmployeeId = employee.Id;
        DateIsSelected = false;
        await GetAvailableDate();
        StateHasChanged();
    }
    
    protected async Task OnKeyDownAsync(KeyboardEventArgs args)
    {
        switch (args.Key)
        {
            case "Enter":
            case "NumpadEnter":
                await Submit();
                break;
            case "Escape":
                Cancel();
                break;
        }
    }

    protected string GetTotalSessionsService(Service service)
    {
        int total = GetTotalSessionByServiceId(service.Id);
        int used = GetTotalUsedService(service.Id);
        int remaing = total - used;
        
        return $"{remaing} de {total} sessões restantes";
    }

    protected async Task OnAddSchedule()
    {
        IsAddschedule = true;
        try
        {
            if (!ValidatedFields()) return;

            CreateSchedules.Add(Create);

            DateIsSelected = false;
            AvailabilitySlotsDto = new();
            AvailableDateDtos = [];
            SelectedEmployee = Employees.FirstOrDefault() ?? new();
            SelectedService = Services.FirstOrDefault() ?? new();

            Create = new()
            {
                ClientId = ClientPackage.ClientId,
                EmployeeId = SelectedEmployee.Id,
                ServiceId = SelectedService.Id,
            };

            await GetAvailableDate();
            await GetServicesByClientPackage();
            IsServiceFullyScheduled(SelectedService.Id);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally
        {
            IsAddschedule = false;
        }
    }
    
    protected async Task OnRemoveSchedule(GenerateScheduleWithEmployeeRequest schedule)
    {
        try
        {
            CreateSchedules.Remove(schedule);
            IsServiceFullyScheduled(schedule.ServiceId);
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }
    
    protected async Task Submit()
    {
        if(IsBusy) return;
        try
        {
            bool? response;
            
            if (Package.TotalSessions > CreateSchedules.Count)
            {
                response = await DialogService.ShowMessageBox("Agendamento", $"Foi definido {CreateSchedules.Count} de {Package.TotalSessions}. Deseja prosseguir da mesma forma?",
                    yesText: "SIM", noText: "NÃO");
                if (response is false)
                    return;
            }
            
            response = await DialogService.ShowMessageBox("Agendamento", "Confirma dados corretos?",
                yesText: "SIM", noText: "NÃO");
            if (response is false)
                return;

            var result = await AppointmentHandler.CreateAsync(new GenerateAutoScheduleRequest()
            {
                Schedules = CreateSchedules
            });

            if (result.IsSuccess)
                Snackbar.Add("Agendamentos efetuados com sucesso!", Severity.Success);
            else
                Snackbar.Add("Não foi possível efetuar os agendamentos!", Severity.Success);
            
            MudDialog.Close(DialogResult.Ok(result.IsSuccess));
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    protected void Cancel() => MudDialog.Cancel();

    #region Private Methods
    private bool ValidatedFields()
    {
        if (Create.EmployeeId == 0)
        {
            Snackbar.Add("Informe um profissional para prosseguir!", Severity.Warning);
            return false;
        }
        
        if (Create.ClientId == 0)
        {
            Snackbar.Add("Informe o cliente para prosseguir!", Severity.Warning);
            return false;
        }
        
        if (Create.ServiceId == 0)
        {
            Snackbar.Add("Informe o serviço para prosseguir!", Severity.Warning);
            return false;
        }

        if (Create.Date == DateTime.MinValue
            || Create.Date == DateTime.MaxValue
            || Create.Date < DateTime.Today)
        {
            Snackbar.Add("Selecione uma data valida para prosseguir!", Severity.Warning);
            return false;
        }

        if (Create.StartTime == TimeOnly.MinValue
            || Create.StartTime == TimeOnly.MaxValue)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }

        if (Create.EndTime == TimeOnly.MinValue
            || Create.EndTime == TimeOnly.MaxValue)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }
        
        if (Create.StartTime > Create.EndTime)
        {
            Snackbar.Add("Selecione um horário valido para prosseguir!", Severity.Warning);
            return false;
        }

        return true;
    }
    private async Task GetPackageAsync()
    {
        try
        {
            var package = await PackageHandler.GetByIdAsync(new GetPackageByIdRequest()
            {
                Id = ClientPackage.PackageId
            });

            if (package is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível carregar o serviço!", Severity.Error);
                NavigationManager.NavigateTo("/packages");
                Cancel();
                return;
            }

            Package = package.Data ?? new();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
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
                Cancel();
            }

            Employees = result.Data ?? [];
            SelectedEmployee = Employees.FirstOrDefault() ?? new Employee();
            Create.EmployeeId = SelectedEmployee.Id;
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
    private async Task GetServicesByClientPackage()
    {
        try
        {
            var packageClient = await ClientPackageHandler.GetByClientIdAsync(new GetActiveClientPackageRequest
            {
                ClientId = ClientPackage.ClientId,
            });

            if (packageClient.Data is null)
            {
                Snackbar.Add($"O cliente não possuí pacotes ativos no momento. ", Severity.Warning);
                NavigationManager.NavigateTo("/clients-packages");
                Cancel();
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
                Cancel();
                return;
            }
            
            Services = result.Data ?? [];
            SelectedService = Services.FirstOrDefault() ?? new Service();
            Create.ServiceId = SelectedService.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    private async Task GetAvailableDate()
    {
        try
        {
            var result = await AppointmentHandler.GetAvailableDatesAsync(new GetAvailableDateRequest
            {
                EmployeeId = SelectedEmployee.Id,
            });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar as datas disponíveis do profíssional!", Severity.Warning);
                NavigationManager.NavigateTo("/appointments");
                Cancel();
                return;
            }

            if (result.Data is null || !result.Data.Any())
            {
                Snackbar.Add("O profíssional não possuí datas disponíveis, abra a agenda para prosseguir!");
                return;
            }
            
            AvailableDateDtos = result.Data
                .Where(x => x.Date >= ClientPackage.StartDate 
                            && x.Date <= ClientPackage.EndDate)
                .ToList() ?? [];
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
                    Date = Create.Date,
                    EmployeeId = SelectedEmployee.Id,
                    ServiceId = SelectedService.Id
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
    private int GetTotalSessionByServiceId(long serviceId)
     => Package.PackageServices
         .Where(x => x.ServiceId == serviceId)
         .Sum(x => x.SessionsPerService);
    
    private int GetTotalUsedService(long serviceId)
        => CreateSchedules.Count(x => x.ServiceId == serviceId);

    private void IsServiceFullyScheduled(long serviceId)
    {
        int total = GetTotalSessionByServiceId(serviceId);
        int used = GetTotalUsedService(serviceId);
        
        IsFullySchedule = used >= total;
    }
    #endregion
   
}