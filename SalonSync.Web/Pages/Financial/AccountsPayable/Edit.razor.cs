using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;

namespace SalonSync.Web.Pages.Financial.AccountsPayable;

public partial class EditAccountPayablePage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAccountPayableHandler AccountHandler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    [Parameter] public string Id { get; set; } = string.Empty;
    public long AccountId { get; set; }
    public bool IsStart { get; set; }
    public bool IsSubmit { get; set; }
    public AccountPayable Payable { get; set; } = new();
    public UpdateAccountPayableRequest InputModel { get; set; } = new();
    #endregion
    
    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            AccountId = long.Parse(Id);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi converter os paramêtros!", Severity.Warning);
            NavigationManager.NavigateTo("/accounts-payable");
            return;
        }
        
        try
        {
            var result = await AccountHandler.GetByIdAsync(new GetAccountPayableByIdRequest
            {
                Id = AccountId
            });

            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar a conta a pagar, Operação cancelada!", Severity.Warning);
                NavigationManager.NavigateTo("/accounts-payable");
                return;
            }
            
            InputModel = new()
            {
                Id = AccountId,
                Description = result.Data?.Description ?? string.Empty,
                SupplierName = result.Data?.SupplierName ?? string.Empty,
                SupplierDocument = result.Data?.SupplierDocument ?? string.Empty,
                Amount = result.Data?.Amount ?? 0,
                DueDate = result.Data?.DueDate ?? DateTime.Now,
            };
        }
        catch (Exception e)
        {
            Snackbar.Add($"Não foi possível inciar os dados! Erro: {e.Message}", Severity.Error);
        }
    }

    #endregion
    
    #region Methods

    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            var result = await AccountHandler.UpdateAsync(InputModel);

            if (!result.IsSuccess && result.Data is null)
            {
                Snackbar.Add(result.Message, Severity.Error);
                return;
            }
            
            Snackbar.Add("Contas a pagar alterada com sucesso!", Severity.Success);
            NavigationManager.NavigateTo("/accounts-payable");
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível alterar a conta a pagar!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    #endregion
}