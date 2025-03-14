using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;

namespace SalonSync.Web.Pages.Financial.CostCenters;

public partial class CreateCenterCostPage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ICostCenterHandler Handler { get; set; } = null!;
    #endregion

    #region Properties
    public bool IsSubmit { get; set; }
    public CreateCostCenterRequest InputModel { get; set; } = new();
    #endregion
    
    #region Methods

    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            var result = await Handler.CreateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Centro de custo criado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/cost-centers");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o centro de custo!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    #endregion
}