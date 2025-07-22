using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Web.Services;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Financial.AccountsPayable;

public partial class CreateAccountPayablePage : ComponentBase
{
    #region Dependencies
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IAccountPayableHandler AccountHandler { get; set; } = null!;
    [Inject] public PaymentService PaymentService { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    #endregion

    #region Properties
    public bool IsSubmit { get; set; }
    public bool IsPayd { get; set; }
    public AccountPayable Payable { get; set; } = new();
    public CreateAccountPayableDetails InputModel { get; set; } = new();
    public List<InstallmentViewModel> InstallmentViewModels { get; set; } = [];
    #endregion

    #region Installments

    private bool _isInstallment = false;

    public bool IsInstallment
    {
        get => _isInstallment;
        set
        {
            if (_isInstallment != value)
            {
                _isInstallment = value;
                GenerateInstallments();
            }
        }
    }
    private int _installmentCount = 1;
    public int InstallmentCount
    {
        get => _installmentCount;
        set
        {
            if (_installmentCount != value)
            {
                _installmentCount = value;
                GenerateInstallments();
            }
        }
    }

    private int _installmentInterval = 30;
    public int InstallmentInterval
    {
        get => _installmentInterval;
        set
        {
            if (_installmentInterval != value)
            {
                _installmentInterval = value;
                GenerateInstallments();
            }
        }
    }

    private bool _isAmountPerInstallment;
    public bool IsAmountPerInstallment
    {
        get => _isAmountPerInstallment;
        set
        {
            if (_isAmountPerInstallment != value)
            {
                _isAmountPerInstallment = value;
                GenerateInstallments();
            }
        }
    }
    public bool IsLoadingInstallment { get; set; }

    #endregion
    
    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        try
        {
            InputModel = new()
            {
                DueDate = DateTime.Today.AddDays(30),
                SupplierDocument = string.Empty,
            };
        }
        catch (Exception e)
        {
            Snackbar.Add($"Não foi possível inciar os dados! Erro: {e.Message}", Severity.Error);
        }
    }

    #endregion
    
    #region Methods

    public async Task OnValidSubmit()
    {
        IsSubmit = true;
        try
        {
            CreateAccountPayableRequest request = new();

            if (InstallmentViewModels.Any())
            {
                request.CreateAccountPayableDetailsList = InstallmentViewModels.Select(x =>
                    new CreateAccountPayableDetails
                    {
                        Description = InputModel.Description + $" PARCELA {x.Number} DE {InstallmentViewModels.Count}",
                        SupplierName = InputModel.SupplierName,
                        SupplierDocument = InputModel.SupplierDocument,
                        Amount = x.Amount,
                        DueDate = x.DueDate
                    }).ToList();
            }
            else
            {
                request.CreateAccountPayableDetailsList.Add(InputModel);
            }
            
            var result = await AccountHandler.CreateAsync(request);

            if (!result.IsSuccess && result.Data is null)
            {
                Snackbar.Add(result.Message, Severity.Error);
                return;
            }
            
            Snackbar.Add("Contas a pagar cadastrada com sucesso!", Severity.Success);
            NavigationManager.NavigateTo("/accounts-payable");
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível lançar a conta a pagar!", Severity.Error);
        }
        finally
        {
            IsSubmit = false;
        }
    }
    
    public void GenerateInstallments()
    {
        IsLoadingInstallment = true;
        if (!IsInstallment || InstallmentCount < 1 || InputModel.Amount <= 0 || InputModel.DueDate == default)
        {
            InstallmentViewModels.Clear();
            return;
        }

        InstallmentViewModels.Clear();
        decimal amountPerInstallment = IsAmountPerInstallment ? InputModel.Amount : InputModel.Amount / InstallmentCount;
        DateTime? dueDate = InputModel.DueDate;

        for (int i = 0; i < InstallmentCount; i++)
        {
            InstallmentViewModels.Add(new InstallmentViewModel
            {
                Number = i + 1,
                DueDate = dueDate,
                Amount = amountPerInstallment
            });

            dueDate = dueDate.Value.AddDays(InstallmentInterval);
        }

        IsLoadingInstallment = false;
    }

    public void OnValueDateChanged(DateTime? date)
    {
        InputModel.DueDate = date;
        GenerateInstallments();
    }
    
    public void OnValueAmountChanged(decimal amount)
    {
        InputModel.Amount = amount;
        GenerateInstallments();
    }
    #endregion
}