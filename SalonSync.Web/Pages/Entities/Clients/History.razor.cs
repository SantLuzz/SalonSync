using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class History : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientPackageHandler Handler { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    #endregion
    
    #region Properties
    [Parameter] public string Id { get; set; } = string.Empty;
    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    public int ClientId { get; set; }
    public List<ClientPackage> Packages { get; set; } = [];
    public ClientPackage? ActivePackage { get; set; }

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        
        ClientId = 0;
        try
        {
            ClientId = int.Parse(Id);
        }
        catch
        {
            Snackbar.Add("Não foi possível converter os parâmetros!", Severity.Error);
            return;
        }
        
        try
        {
            var packages = await Handler.GetAllByClientIdAsync(new GetAllPackagesByClientIdRequest
            {
                ClientId = this.ClientId
            });
            
            if (!packages.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar o histórico do cliente!", Severity.Error);
                NavigationManager.NavigateTo("/clients");
                return;
            }

            Packages = packages.Data ?? [];

            var active = await ClientPackageHandler.GetByClientIdAsync(new GetActiveClientPackageRequest
            {
                ClientId = ClientId
            });

            ActivePackage = active.Data;
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally{IsBusy = false;}
    }

    #endregion
}