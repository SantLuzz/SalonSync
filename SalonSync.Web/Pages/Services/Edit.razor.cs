using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Services;
using SalonSync.Core.Requests.Services;

namespace SalonSync.Web.Pages.Services;

public partial class Edit : ComponentBase
{
     #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IServiceHandler Handler { get; set; } = null!;
    
    #endregion
    
    #region Properties

    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    
    public UpdateServiceRequest InputModel { get; set; } = new();
    public int serviceId { get; set; }
    [Parameter] public string Id { get; set; } = string.Empty;

    #endregion

    #region overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        
        serviceId = 0;
        try
        {
            serviceId = int.Parse(Id);
        }
        catch
        {
           Snackbar.Add("Não foi possível converter os parâmetros!", Severity.Error);
           return;
        }

        try
        {
            var service = await Handler.GetByIdAsync(new GetServiceByIdRequest()
            {
                Id = serviceId
            });

            if (service is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível carregar o serviço!", Severity.Error);
                NavigationManager.NavigateTo("/services");
                return;
            }

            InputModel = new()
            {
                Id = serviceId,
                UserId = service.Data.UserId,
                Name = service.Data.Name,
                Duration = service.Data.Duration,
                Price = service.Data.Price,
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally{IsBusy = false;}
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
                Snackbar.Add("Serviço alterado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/services");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível alterar o serviço!", Severity.Error);
        }
        finally{IsSubmit = false;}
    }
    
    
    #endregion
}