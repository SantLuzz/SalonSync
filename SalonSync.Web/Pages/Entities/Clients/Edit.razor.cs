using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Requests.Packages;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class Edit : ComponentBase
{
    #region Dependencies

    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientHandler Handler { get; set; } = null!;
    [Inject] public IPackageHandler PackageHandler { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    #endregion
    
    #region Properties
    [Parameter] public string Id { get; set; } = string.Empty;
    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    public CustomClientViewModel InputModel { get; set; } = new();
    public int ClientId { get; set; }
    public List<PackageDto> PackageDtos { get; set; } = [];

    #endregion

    #region overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        await GetPackageAsync();
        await GetClientAsync();
        IsBusy = false;
    }

    #endregion
    
    #region Methods

    public bool CheckMonthlyCustomer()
    {
        return InputModel.UpdateRequest?.Type == EClientType.Monthly;
    }
    
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

            // if (InputModel.UpdateRequest.Type == EClientType.Common 
            //     && InputModel.ClientPackage is not null)
            //     await OnInactivePackageAsync();
            
            
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
    
    

    private async Task GetClientAsync()
    {
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

            var active = await ClientPackageHandler.GetByClientIdAsync(new GetActiveClientPackageRequest
            {
                ClientId = ClientId
            });
            
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
                },
                ClientPackage = active.Data ?? null,
                CreateClientPackageRequest = new()
                {
                    ClientId = client.Data.Id,
                    PackageId = active.Data?.PackageId ?? (PackageDtos.FirstOrDefault()?.Id ?? 0),
                    StartDate = active.Data?.StartDate,
                    EndDate = active.Data?.EndDate
                }
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }

    private async Task GetPackageAsync()
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
            
            PackageDtos = packages.Data?.Select(x => 
              new PackageDto
              {
                  Id = x.Id,
                  UserId = x.UserId,
                  Name = x.Name,
                  Price = x.Price,
                  PackageServiceDtos = []
              }).ToList() ?? [];
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
    }
    #endregion
}