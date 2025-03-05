using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Models.Services;
using SalonSync.Core.Requests.Services;

namespace SalonSync.Web.Pages.Services;

public partial class List : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IServiceHandler Handler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public string SearchTerm { get; set; } = string.Empty;
    public bool IsStarted { get; set; }
    public List<Service> Services { get; set; } = [];
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
        => await GetAllServicesAsync();

    #endregion

    #region Methods
    public async void OnDeleteButtonClickedAsync(long id, string name)
    {
        var result = await DialogService
            .ShowMessageBox("Atenção", $"Ao prosseguir o serviço {name} será excluído. " +
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
            await Handler.DeleteAsync(new DeleteServiceRequest() { Id = id });
            Services.RemoveAll(x => x.Id == id);
            Snackbar.Add($"Serviço {name} excluído com sucesso!", Severity.Success);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }
    
    public Func<Service, bool> Filter => package =>
    {
        if (string.IsNullOrWhiteSpace(SearchTerm))
            return true;
        
        if (package.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        if(package.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
            return true;
        
        return false;
    };
    
    private async Task GetAllServicesAsync()
    {
        IsStarted = true;
        try
        {
            var request = new GetAllServicesRequest()
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var result = await Handler.GetAllAsync(request);

            if (result is { IsSuccess: true })
                Services = result.Data ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }

    }
    #endregion
}