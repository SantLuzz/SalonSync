using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class Edit : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientHandler Handler { get; set; } = null!;
    
    #endregion
    
    #region Properties

    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    
    public CustomClientViewModel InputModel { get; set; } = new();
    public int ClientId { get; set; }
    [Parameter] public string Id { get; set; } = string.Empty;

    #endregion

    #region overrides

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
            var client = await Handler.GetByIdAsync(new GetClientByIdRequest
            {
                Id = ClientId
            });

            if (client is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível carregar o cliente!", Severity.Error);
                NavigationManager.NavigateTo("/clients");
                return;
            }

            InputModel = new()
            {
                UpdateRequest = new()
                {
                    Id = ClientId,
                    Name = client.Data.Name,
                    Birthdate = client.Data.Birthdate,
                    Email = client.Data.Email,
                    LastProcedure = client.Data.LastProcedure,
                    Observations = client.Data.Observations,
                    Phone = client.Data.Phone,
                    PreferredService = client.Data.PreferredService,
                    Type = client.Data.Type,
                    UserId = client.Data.UserId,
                },
                ImageRequest = new()
                {
                    Id = client.Data.Id,
                    UrlImage = client.Data.Image
                }
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
        try
        {
            var result = await Handler.UpdateAsync(InputModel.UpdateRequest);

            if (result.IsSuccess)
            {
                Snackbar.Add("Cliente alterado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/clients");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o cliente!", Severity.Error);
        }
    }

    
    
    public async Task OnFileUploadAsync(string base64)
    {
        try
        {
            InputModel.ImageRequest.Base64Image = base64;
            
            var result = await Handler.UpdateImageAsync(InputModel.ImageRequest);

            if (!result.IsSuccess)
                Snackbar.Add("Não foi possível alterar a imagem!", Severity.Error);
            else
                Snackbar.Add("Imagem do cliente alterada com sucesso!", Severity.Success);

            InputModel.ImageRequest.UrlImage = result.Data.Image;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao processar a imagem: {ex.Message}", Severity.Error);
        }
    }
    
    #endregion
}