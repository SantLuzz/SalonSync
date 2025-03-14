using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Requests.Packages;
using SalonSync.Web.Common;

namespace SalonSync.Web.Pages.ClientsPackages;

public partial class ListClientPackagePage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    [Inject] public IPackageHandler PackageHandler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public bool IsSearching { get; set; }
    public List<PackageDto> Packages { get; set; } = [];
    public PackageDto SelectedPackage { get; set; } = new();
    public List<ClientPackageDto> ClientPackageDtos { get; set; } = [];

    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsStarted = true;
        await GetPackagesAsync();
        await GetClientsAsync();
        IsStarted = false;
    }
         
    
    #endregion
    
    #region Methods
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o cliente {name} terá o pacote encerrado. " +
                $"Esta é uma ação irreversível, deseja prosseguir?", "Inativar", cancelText: "Cancelar");
    
        if (result is true)
            await OnDeleteAsync(id, name);
    
        StateHasChanged();
    }
    
    private async Task OnDeleteAsync(long id, string name)
    {
        IsStarted = true;
        try
        {
            var result = await ClientPackageHandler
                .InactiveAsync(new InactiveClientPackageRequest
                {
                    Id = id,
                });

            if (!result.IsSuccess)
                Snackbar.Add(result.Message, Severity.Error);
            
            ClientPackageDtos.RemoveAll(x => x.Id == id);
            Snackbar.Add("Pacote inativado com sucesso!", Severity.Success);
            
            StateHasChanged();
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível inativar o pacote do cliente!", Severity.Error);
        }
        finally{IsStarted = false;}
    }

    public async void OnPackageValueChanged(PackageDto package)
    {
        IsSearching = true;
        SelectedPackage = package;
        await GetClientsAsync();
        IsSearching = false;
        StateHasChanged();
    }
    
    public Func<ClientPackageDto, bool> Filter => client =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (client.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if(client.ClientName.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
    
        if (client.ClientId.ToString() == SearchTerm)
            return true;

        return false;
    };
    
    private async Task GetPackagesAsync()
    {
        try
        {
            var packages = await PackageHandler
                .GetAllAsync(new GetAllPackagesRequest());

            if (packages.IsSuccess && packages.Data is null)
            {
                Snackbar.Add("Não foi possível carregar os pacotes!", Severity.Error);
                NavigationManager.NavigateTo("/packages");
                return;
            }
            
            Packages = packages.Data?.Select(x => 
                new PackageDto
                {
                    Id = x.Id,
                    UserId = x.UserId,
                    Name = x.Name,
                    Price = x.Price,
                    PackageServiceDtos = []
                }).ToList() ?? [];
            
            SelectedPackage = Packages.FirstOrDefault() ?? new ();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }
    
    private async Task GetClientsAsync()
    {
        try
        {
            var request = new GetClientsPackagesByPackageIdRequest()
            {
                PageNumber = 1,
                PageSize = 1000,
                PackageId = SelectedPackage.Id
            };
            
            var result = await ClientPackageHandler.GetByPackageIdAsync(request);
    
            if (result.IsSuccess)
                ClientPackageDtos = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
    
    }
    #endregion
}