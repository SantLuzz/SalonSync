using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.DTOs;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Web.Common;
using SalonSync.Web.Services;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Financial.Commissions
{
    public partial class ListCommission : ComponentBase
    {
        #region Dependencies
        [Inject] public ISnackbar Snackbar { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public ICommissionHandler Handler { get; set; } = null!;
        [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
        [Inject] public IDialogService DialogService { get; set; } = null!;
        [Inject] public PaymentService PaymentService { get; set; } = null!;
        #endregion

        #region properties
        public bool IsStarted { get; set; }
        public string SearchTerm { get; set; } = string.Empty;
        public List<UnpaidCommissionDto> Commissions { get; set; } = [];
        #endregion

        #region override methods
        protected override async Task OnInitializedAsync()
        {
            IsStarted = true;
            await GetCommissionAsync();
            IsStarted = false;
        }
        #endregion

        #region methods
        private async Task GetCommissionAsync()
        {
            try
            {
                var initialDate = DateTime.Now.GetFirstDay();
                var endDate = DateTime.Now.GetLastDay();

                var request = new GetUnpaidCommissionInPeriodRequest
                {
                    StartDate = initialDate,
                    EndDate = endDate
                };

                var result = await Handler.GetUnpaidAsync(request);

                if (result is { IsSuccess: true })
                    Commissions = result.Data ?? [];
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        public Func<UnpaidCommissionDto, bool> Filter => commission =>
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;

            if (commission.EmployeeId.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;

            if (commission.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;

            if (commission.Commission.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;

            if (commission.Total.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        };

        public async Task PayCommission(UnpaidCommissionDto unpaidCommission)
        {
            try
            {
                if (unpaidCommission.EmployeeId == 0)
                {
                    Snackbar.Add("Não é possível efetuar o pagamento da comissão para o profissional informado, efetue manualmente!", Severity.Error);
                    return;
                }

                var response = await DialogService.ShowMessageBox("Pagamento", "Deseja efetuar o lançamento no caixa?",
                    yesText: "SIM", noText: "NÃO");

                if (response is false) return;

                var payment = await PaymentService.GetPaymentDetailsAndPay(new PaymentDetailsViewModel
                {
                    Amount = unpaidCommission.TotalPayable,
                    Description = GetPaymentDescription(unpaidCommission),
                    EntityType = EEntityType.Comission,
                    EntityId = unpaidCommission.EmployeeId,
                    PaidOrReceivedAt = DateTime.Now,
                    CenterType = ECostCenterType.Expense
                });

                if(payment is null || payment?.Id == 0)
                {
                    Snackbar.Add("Pagamento cancelado!", Severity.Warning);
                    return;
                }

                var result = await Handler.CreateAsync(new CreateCommissionRequest
                {
                    EmployeeId = unpaidCommission.EmployeeId,
                    Date = DateTime.Now,
                    Amount = unpaidCommission.TotalPayable
                });

                Snackbar.Add("Comissão paga com sucesso!", Severity.Success);
                await GetCommissionAsync();
            }
            catch (Exception e)
            {
                Snackbar.Add("Não foi possível efetuar o pagamento da comissão!", Severity.Error);
            }
        }

        private string GetPaymentDescription(UnpaidCommissionDto unpaidCommission)
            => $"Pagamento da comissão do profissional {unpaidCommission.Name} ({unpaidCommission.EmployeeId}) " +
            $"no valor referente ao mês {DateTime.Today.ToString("MMMM")}";

        #endregion


    }
}
