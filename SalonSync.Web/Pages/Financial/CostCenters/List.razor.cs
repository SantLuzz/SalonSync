using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;

namespace SalonSync.Web.Pages.Financial.CostCenters;

public partial class ListCostCenterPage : ComponentBase
{
     #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ICostCenterHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<CostCenter> CostCenters { get; set; } = [];
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
        => await GetCostCenterAsync();

    #endregion

    #region Methods
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o Centro de custo {name} será excluído. " +
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
            var result = await Handler.DeleteAsync(new DeleteCostCenterRequest() { Id = id });

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    CostCenters.RemoveAll(x => x.Id == id);
                    Snackbar.Add($"Centro de custo {name} excluído!", Severity.Success);
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
    
    public Func<CostCenter, bool> Filter => costCenter =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (costCenter.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if(costCenter.Title.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (costCenter.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    };
    
    private async Task GetCostCenterAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetAllCostCentersRequest()
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var result = await Handler.GetAllAsync(request);

            if (result is { IsSuccess: true })
                CostCenters = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }

    }
    #endregion
}