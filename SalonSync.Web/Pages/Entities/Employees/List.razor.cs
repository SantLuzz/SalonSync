using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Entities.Employees;

public partial class ListPage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IEmployeeHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<EmployeeViewModel> Employees { get; set; } = [];
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
        => await GetEmployeesAsync();

    #endregion

    #region Methods
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o funcionário {name} será excluído. " +
                $"Esta é uma ação irreversível, deseja prosseguir?", "Excluir", cancelText: "Cancelar");

        if (result is true)
            await OnDeleteAsync(id, name);

        StateHasChanged();
    }

    private async Task OnDeleteAsync(long id, string name)
    {
        IsStarted = true;
        try
        {
            await Handler.DeleteAsync(new DeleteEmployeeRequest() { Id = id });
            Employees.RemoveAll(x => x.Id == id);
            Snackbar.Add($"Funcionário {name} excluído!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }
    
    public Func<EmployeeViewModel, bool> Filter => employee =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (employee.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if(employee.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (employee.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (employee.Phone?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;

        if (employee.Position?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;

        if (employee.Commission == Convert.ToDecimal(SearchTerm))
            return true;

        if (employee.AdmissionFormatted == Convert.ToDateTime(SearchTerm).ToString("dd/MM/yyyy"))
            return true;
        
        return false;
    };
    
    private async Task GetEmployeesAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetAllEmployeesRequest()
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var result = await Handler.GetAllAsync(request);

            if (result is { IsSuccess: true, Data: not null })
            {
                Employees = result.Data.Select(x => new EmployeeViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Email = x.Email,
                    Phone = x.Phone,
                    Position = x.Position,
                    Admission = x.Admission,
                    Commission = x.Commission
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }

    }
    #endregion
}