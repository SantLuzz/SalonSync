using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Requests.Services;

namespace SalonSync.Web.Pages.Services;

public partial class Create : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IServiceHandler Handler { get; set; } = null!;
    
    #endregion
    
    #region Properties
    public bool IsSubmit { get; set; }
    public CreateServiceRequest InputModel { get; set; } = new();
    

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
                Snackbar.Add("Serviço criado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/services");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o Serviço!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    

    #endregion
}