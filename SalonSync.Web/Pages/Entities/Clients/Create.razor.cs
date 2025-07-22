using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Clients;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class Create : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientHandler Handler { get; set; } = null!;
    
    #endregion
    
    #region Properties
    public bool IsSubmit { get; set; }
    public CreateClientRequest InputModel { get; set; } = new();
    

    #endregion

    #region Methods

    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            var result = await Handler.CreateAsync(InputModel);

            if (result.IsSuccess)
            {
                Snackbar.Add("Cliente criado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/clients");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o cliente!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    

    #endregion
}