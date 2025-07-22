using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Web.Common;
using SalonSync.Web.Handlers;
using SalonSync.Web.Services;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Financial.AccountsPayable;

public partial class ListAccountPayablePage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAccountPayableHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public PaymentService PaymentService { get; set; }
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<AccountPayable> AccountPayables { get; set; } = [];
    public EAccountStatus AccountStatus { get; set; } = EAccountStatus.Open;
    public DateTime? Date { get; set; }
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        Date = DateTime.Today.GetFirstDay();
        await GetAccountPayableAsync();
    }

    #endregion

    #region Methods

    public async void OnSearchAsync()
    {
        await GetAccountPayableAsync();
        StateHasChanged();
    }
    
    public async void OnDeleteButtonClickedAsync(long id)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir a conta {id} será excluída. " +
                $"Esta é uma ação irreversível, deseja prosseguir?", "Excluir", cancelText: "Cancelar");

        if (result is true)
            await OnDeleteAsync(id);

        StateHasChanged();
    }

    private async Task OnDeleteAsync(long id)
    {
        IsStarted = true;
        try
        {
            var result = await Handler.DeleteAsync(new DeleteAccountPayableRequest() { Id = id });

            if (result.IsSuccess)
            {
                if (result.Data is not null)
                {
                    AccountPayables.RemoveAll(x => x.Id == id);
                    Snackbar.Add($"Conta {id} excluída!", Severity.Success);
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
    
    public Func<AccountPayable, bool> Filter => account =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (account.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (account.Description.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (account.SupplierName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (account.SupplierDocument.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    };
    
    private async Task GetAccountPayableAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetAccountPayableByPeriodRequest
            {
                PageNumber = 1,
                PageSize = 1000,
                StartDate = Date.Value.GetFirstDay(),
                EndDate = Date.Value.GetLastDay(),
                AccountStatus = this.AccountStatus
            };
            var result = await Handler.GetByPeriodAsync(request);

            if (result is { IsSuccess: true })
                AccountPayables = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }
    
    public async Task PayAccount(AccountPayable payable)
    {
        try
        {
            if (payable.Id == 0)
            {
                Snackbar.Add("Não é possível efetuar o pagamento para a conta informada, efetue manualmente!", Severity.Error);
                return;
            }
            
            var response = await DialogService.ShowMessageBox("Pagamento", "Deseja efetuar o lançamento no caixa?",
                yesText: "SIM", noText: "NÃO");
        
            if (response is false) return;
            
            var payment = await PaymentService.GetPaymentDetailsAndPay(new PaymentDetailsViewModel
            {
                Amount = payable.Amount,
                Description = GetPaymentDescription(payable),
                EntityType = EEntityType.AccountPayable,
                EntityId = payable.Id,
                PaidOrReceivedAt = DateTime.Now,
                CenterType = ECostCenterType.Expense
            });
        
            var result = await Handler.CloseAsync(new CloseAccountPayableRequest
            {
                Id = payable.Id,
                PaymentDate = payment?.PaidOrReceivedAt ?? DateTime.Now,
            });
            
            Snackbar.Add("Conta paga com sucesso!", Severity.Success);
            await GetAccountPayableAsync();
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível efetuar o pagamento da conta!", Severity.Error);
        }
    }

    private string GetPaymentDescription(AccountPayable payable)
        => $"Pagamento da conta #{payable.Id.ToString().PadLeft(6,'0')} para o fornecedor {(string.IsNullOrWhiteSpace(payable.SupplierDocument)
            ? "" : $"({Masks.ApplyDocumentMask(payable.SupplierDocument)}) ")}{payable.SupplierName}";

    #endregion
}