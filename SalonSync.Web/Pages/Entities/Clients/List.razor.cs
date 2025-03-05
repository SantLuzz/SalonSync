using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Web.Common;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class ListPage : ComponentBase
{
  #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<Client> Clients { get; set; } = [];
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
        => await GetClientsAsync();

    #endregion

    #region Methods
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o cliente {name} será excluído. " +
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
            await Handler.DeleteAsync(new DeleteClientRequest() { Id = id });
            Clients.RemoveAll(x => x.Id == id);
            Snackbar.Add($"Cliente {name} excluído com sucesso!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }
    
    public Func<Client, bool> Filter => client =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (client.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if(client.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (client.Email.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;

        if (client.Phone?.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
            return true;
        
        return false;
    };
    
    private async Task GetClientsAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetAllClientsRequest()
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var result = await Handler.GetAllAsync(request);

            if (result is { IsSuccess: true, Data: null })
                Clients =  [];
            else
            {
                Clients = result.Data.Select(x =>
                {
                    x.Phone = Masks.ApplyPhoneMask(x.Phone ?? string.Empty);
                    return x;
                }).ToList();
            }
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }

    }
    #endregion
}