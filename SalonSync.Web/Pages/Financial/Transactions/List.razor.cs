using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Requests.Financial.Transactions;

namespace SalonSync.Web.Pages.Financial.Transactions;

public partial class ListTransactionPage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ITransactionHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<Transaction> Transactions { get; set; } = [];
    public DateTime? Date { get; set; }
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        Date = DateTime.Today;
        await GetTransactionsAsync();
    }

    #endregion

    #region Methods

    public async void OnSearchAsync()
    {
        await GetTransactionsAsync();
        StateHasChanged();
    }
        
    
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o lançamento {id} será excluído. " +
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
            var result = await Handler.DeleteAsync(new DeleteTransactionRequest() { Id = id });

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    Transactions.RemoveAll(x => x.Id == id);
                    Snackbar.Add($"Transação {id} excluída!", Severity.Success);
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
    
    public Func<Transaction, bool> Filter => transaction =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (transaction.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (transaction.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    };
    
    private async Task GetTransactionsAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetTransactionsByPeriodRequest()
            {
                PageNumber = 1,
                PageSize = 1000,
                StartDate = Date,
            };
            var result = await Handler.GetByDayAsync(request);

            if (result is { IsSuccess: true })
                Transactions = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }

    }
    #endregion
}