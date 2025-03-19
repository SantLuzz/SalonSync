using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Requests.Financial.Transactions;

namespace SalonSync.Web.Pages.Financial.Transactions;

public partial class EditTransactionPage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ITransactionHandler Handler { get; set; } = null!;
    [Inject] public ICostCenterHandler CostCenterHandler { get; set; } = null!;
    #endregion

    #region Properties
    [Parameter] public string Id { get; set; } = string.Empty;
    public long TransactionId { get; set; }
    public bool IsSubmit { get; set; }
    public List<CostCenter> CostCenters { get; set; } = [];
    public UpdateTransactionRequest InputModel { get; set; } = new();
    #endregion
    
    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            TransactionId = long.Parse(Id);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi converter os paramêtros!", Severity.Warning);
            NavigationManager.NavigateTo("/transactions");
            return;
        }
        
        
        try
        {
            var result = await CostCenterHandler.GetAllAsync(new GetAllCostCentersRequest());

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar os centros de custos, Operação cancelada!", Severity.Warning);
                NavigationManager.NavigateTo("/transactions");
                return;
            }

            CostCenters = result?.Data ?? [];

            var transaction = await Handler.GetByIdAsync(new GetTransactionByIdRequest
            {
                Id = TransactionId
            });
            
            if (!transaction.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar o lançamento, Operação cancelada!", Severity.Warning);
                NavigationManager.NavigateTo("/transactions");
                return;
            }
            
            InputModel = new()
            {
                Id = TransactionId,
                Description = transaction.Data?.Description ?? string.Empty,
                CostId = transaction.Data?.CostCenterId ?? 0,
                PaidOrReceivedAt = transaction.Data?.PaidOrReceivedAt ?? DateTime.Now,
                Amount = transaction.Data?.Amount ?? 0,
                PaymentType = transaction.Data?.PaymentType ?? EPaymentType.Cash
            };
        }
        catch (Exception e)
        {
            Snackbar.Add($"Não foi possível carregar os centros de custo! Erro: {e.Message}", Severity.Error);
        }
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
                Snackbar.Add("Lançamento efetuado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/transactions");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível efetuar o lançamento!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }

    private bool ValidateInput()
    {
        if (InputModel.CostId == 0)
        {
            Snackbar.Add("Informe o centro de custo do lançamento!", Severity.Warning);
            return false;
        }
        
        if (string.IsNullOrWhiteSpace(InputModel.Description))
        {
            Snackbar.Add("Informe a descrição do lançamento!", Severity.Warning);
            return false;
        }
        
        if (InputModel.Amount == 0)
        {
            Snackbar.Add("Informe o valor do lançamento!", Severity.Warning);
            return false;
        }
        
        if (!InputModel.PaidOrReceivedAt.HasValue)
        {
            Snackbar.Add("Informe a data de pagamento do lançamento!", Severity.Warning);
            return false;
        }
        
        return true;
    }
    #endregion
}