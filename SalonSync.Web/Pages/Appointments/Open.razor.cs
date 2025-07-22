using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Core.Requests.Entities.Employees;

namespace SalonSync.Web.Pages.Appointments;

public partial class OpenAppointmentsPage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAppointmentHandler Handler { get; set; } = null!;
    [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public bool IsStarted { get; set; }
    public bool IsSubmit { get; set; }
    public List<Employee> Employees { get; set; } = [];
    public OpenAppointmentByEmployeeRequest InputModel { get; set; } = new();
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsStarted = true;
        InputModel.StartDate = DateTime.Today;
        InputModel.EndDate = DateTime.Today.AddDays(30);
        await GetEmployeesAsync();
        IsStarted = false;
    }
    #endregion
    
    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            var dialog = await DialogService.ShowMessageBox("Atenção",
                "Deseja abrir a agenda do profissíonal selecionado?",
                yesText: "SIM", noText: "NÃO");

            if (dialog is false)
            {
                Snackbar.Add("Operação cancelada!", Severity.Warning);
                return;
            }
            
            var result = await Handler.OpenByEmployeeIdAsync(InputModel);

            if (!result.IsSuccess && result.Data is null)
            {
                Snackbar.Add(result.Message, Severity.Error);
                return;
            }
            
            Snackbar.Add("Agenda do profissional aberta!", Severity.Success);
            NavigationManager.NavigateTo("/appointments");
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível abrir a agenda dos profissional!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
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
            }

            Employees = result.Data ?? [];
            InputModel.EmployeeId = Employees.FirstOrDefault()?.Id ?? 0;
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }
}