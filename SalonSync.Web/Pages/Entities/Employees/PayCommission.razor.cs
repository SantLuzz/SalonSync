using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Web.Services;

namespace SalonSync.Web.Pages.Entities.Employees;

public partial class PayComissionPage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public PaymentService PaymentService { get; set; } = null!;
    [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
    #endregion
    
    #region Properties
    protected List<Employee> Employees { get; set; } = [];
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        await GetEmployeesAsync();
    }

    #endregion
    
    #region Methods

    protected async Task PayCommission(Employee employee)
    {
        
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
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    }

    private async Task GetCommissionAsync()
    {
        
    }
    #endregion
}