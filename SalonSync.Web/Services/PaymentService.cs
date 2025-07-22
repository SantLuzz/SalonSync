using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Transactions;
using SalonSync.Web.Components.Dialogs;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Services;

public class PaymentService(ITransactionHandler handler, 
    ISnackbar snackbar,
    IDialogService dialogService)
{
    public async Task<Transaction?> GetPaymentDetailsAndPay(PaymentDetailsViewModel details)
    {
        try
        {
            var parameters = new DialogParameters
            {
                { "Label", "Informe os detalhes do pagamento" },
                { "Details", details }
            };
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true,
                Position = DialogPosition.Center
            };

            var dialog = await dialogService
                .ShowAsync<PaymentDetailsDialog>("Novo Pagamento", parameters, options);

            var result = await dialog.Result;
            if (result is { Canceled: true })
            {
                snackbar.Add("Modo de pagamento não confirmado, operação cancelada!", Severity.Warning);
                return null;
            }

            var detailsPay = (PaymentDetailsViewModel)(result?.Data ?? new());

            var create = new CreateTransactionRequest
            {
                Description = detailsPay.Description,
                Amount = detailsPay.Amount,
                CostId = detailsPay.CostCenterId,
                PaidOrReceivedAt = detailsPay.PaidOrReceivedAt,
                EntityId = detailsPay.EntityId,
                EntityType = detailsPay.EntityType,
                PaymentType = detailsPay.PaymentType,
            };
            
            return await Pay(create);
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi possível carregar os detalhes para o lançamento no caixa: {e.Message}", Severity.Error);
            return null;
        }
    }
    
    private  async Task<Transaction?> Pay(CreateTransactionRequest request)
    {
        try
        {
            if (request.CostId == 0)
            {
                snackbar.Add("Informe o centro de custo para prosseguir!", Severity.Error);
                return null;
            }
            
            var transaction = await handler.CreateAsync(request);

            if (!transaction.IsSuccess)
            {
                snackbar.Add("Não foi possível efetuar o pagamento, efetue o mesmo manualmente!", Severity.Success);
                return null;
            }
            
            snackbar.Add("Pagamento efetuado com sucesso!", Severity.Success);
            return transaction.Data;
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi possível efetuar o lançamento no caixa: {e.Message}", Severity.Error);
            return null;
        }
    }

    public async Task<bool> Refound(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await handler.DeleteAsync(request);

            if (!transaction.IsSuccess)
            {
                snackbar.Add("Não foi possível cancelar o lançamento no caixa!", Severity.Success);
                return false;
            }
            
            snackbar.Add("Lançamento cancelado com sucesso!", Severity.Success);
            return true;
        }
        catch (Exception e)
        {
            snackbar.Add($"Não foi possível cancelar o lançamento no caixa: {e.Message}", Severity.Error);
            return false;
        }
    }
}