using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;

namespace SalonSync.Web.Pages.Financial.CostCenters;

public partial class EditCostCenterPage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ICostCenterHandler Handler { get; set; } = null!;
    #endregion

    #region Properties

    [Parameter] public string Id { get; set; } = string.Empty;
    public int CostCenterId { get; set; }
    public bool IsSubmit { get; set; }
    public bool IsBusy { get; set; }
    public UpdateCostCenterRequest InputModel { get; set; } = new();
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        
        CostCenterId = 0;
        try
        {
            CostCenterId = int.Parse(Id);
        }
        catch
        {
           Snackbar.Add("Não foi possível converter os parâmetros!", Severity.Error);
           return;
        }

        try
        {
            var cost = await Handler.GetByIdAsync(new GetCostCenterByIdRequest()
            {
                Id = CostCenterId
            });

            if (cost is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível carregar o Centro de Custo!", Severity.Error);
                NavigationManager.NavigateTo("/cost-centers");
                return;
            }

            InputModel = new()
            {
                Id = CostCenterId,
                Title = cost.Data?.Title ?? string.Empty,
                Description = cost.Data?.Description ?? string.Empty,
                Type = cost.Data?.Type ?? ECostCenterType.Income,
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally{IsBusy = false;}
    }

    #endregion
    
    #region Methods

    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            var result = await Handler.UpdateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Centro de custo alterado com sucesso!", Severity.Success);
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