using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Requests.Entities.Clients;
using SalonSync.Core.Requests.Packages;
using SalonSync.Web.Services;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.ClientsPackages;

public partial class CreatePage : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IClientPackageHandler ClientPackageHandler { get; set; } = null!;
    [Inject] public IPackageHandler PackageHandler { get; set; } = null!;
    [Inject] public IClientHandler ClientHandler { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public PaymentService PaymentService { get; set; } = null!;
    #endregion

    #region Properties

    public bool IsStarted { get; set; }
    public bool IsSubmit { get; set; }
    [Parameter] public string id { get; set; } = string.Empty;
    public long PackageId { get; set; }
    public List<Client> Clients { get; set; } = [];
    public CreateClientPackageRequest InputModel { get; set; } = new();
    public Package Package { get; set; } = new();
    #endregion

    protected override async Task OnInitializedAsync()
    {
        try
        {
            PackageId = long.Parse(id);
        }
        catch (Exception e)
        {
           Snackbar.Add("Não foi possível converter os parâmetros", Severity.Error);
           NavigationManager.NavigateTo("/clients-packages");
           return;
        }

        IsStarted = true;
        try
        {
            var package = await PackageHandler.GetByIdAsync(new GetPackageByIdRequest
            {
                Id = PackageId
            });

            if (!package.IsSuccess)
            {
                Snackbar.Add("Não foi possível carregar o pacote informado!", Severity.Error);
                NavigationManager.NavigateTo("/clients-packages");
                return;
            }
            
            Package = package.Data ?? new();
            
            var request = new GetAllClientsRequest()
            {
                PageNumber = 1,
                PageSize = 1000
            };
            var result = await ClientHandler.GetAllAsync(request);

            if (result.IsSuccess)
                Clients =  result.Data ??[];
            
            InputModel.ClientId = Clients.FirstOrDefault()?.Id ?? 0;
            InputModel.PackageId = PackageId;
            InputModel.StartDate = DateTime.Now;
            InputModel.EndDate = DateTime.Now.AddDays(30);
        }
        catch (Exception ex)
        {
            Snackbar.Add(ex.Message, Severity.Error);
        }
        finally { IsStarted = false; }
    }

    public async Task OnValidSubmit()
    {
        try
        {
            var result = await ClientPackageHandler.CreateAsync(InputModel);
            if (!result.IsSuccess)
            {
                Snackbar.Add("Não foi possível adicionar o pacote!", Severity.Error);
                return;                
            }
            
            Snackbar.Add("Pacote vinculado com sucesso ao cliente!", Severity.Success);
            
            
            var response = await DialogService.ShowMessageBox("Pagamento", "Deseja efetuar o lançamento no caixa?",
                yesText: "SIM", noText: "NÃO");

            if (response is true)
            {
                var payment = await PaymentService.GetPaymentDetailsAndPay(new PaymentDetailsViewModel
                {
                    Amount = Package?.Price ?? 0,
                    Description = GetPaymentDescription(),
                    EntityType = EEntityType.Packages,
                    EntityId = result.Data?.Id ?? 0,
                    PaidOrReceivedAt = DateTime.Now,
                    CenterType = ECostCenterType.Income
                });
            }
            
            NavigationManager.NavigateTo("/clients-packages");
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao definir o pacote para o cliente: {ex.Message}", Severity.Error);
        }
    }

    private string GetPaymentDescription()
    {
        var clientName = Clients.FirstOrDefault(x => x.Id == InputModel.ClientId)?.Name ?? string.Empty;

        return $"Pagamento da mensalidade referente ao pacote #{PackageId.ToString().PadLeft(6, '0')} ({Package.Name}) " +
               $"para o cliente ({InputModel.ClientId}) {clientName}";
    }
}