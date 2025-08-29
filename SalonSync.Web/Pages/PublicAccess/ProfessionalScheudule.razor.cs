using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Appointments;
using SalonSync.Web.Services;

namespace SalonSync.Web.Pages.PublicAccess
{
    public class ProfessionalScheudulePage : ComponentBase
    {
        #region Dependencies

        [Inject] public ISnackbar Snackbar { get; set; } = null!;
        [Inject] public NavigationManager NavigationManager { get; set; } = null!;
        [Inject] public IAppointmentHandler Handler { get; set; } = null!;
        [Inject] public IEmployeeHandler EmployeeHandler { get; set; } = null!;
        [Inject] public ControlAppointmentsClientService ClientService { get; set; } = null!;
        [Inject] public PaymentService PaymentService { get; set; } = null!;
        [Inject] public IDialogService DialogService { get; set; } = null!;
        #endregion

        #region Properties
        public bool IsStarted { get; set; }
        public List<AppointmentDto> Appointments { get; set; } = [];
        public EntityDto Employee { get; set; } = new();
        public DateTime? Date { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public Guid Id { get; set; }
        #endregion

        #region Overrides
        protected override async Task OnInitializedAsync()
        {
            IsStarted = true;
            Date = DateTime.Today;
            await GetAppointmentsAsync();
            ShowEmployee();
            IsStarted = false;
        }

        #endregion

        #region Methods

        public async void OnSearchAsync()
        {
            await GetAppointmentsAsync();
            StateHasChanged();
        }

        private async Task GetAppointmentsAsync()
        {
            try
            {
                var request = new GetAppointmentByEmployeeCodeRequest()
                {
                    Code = Id,
                    Date = Date ?? DateTime.Today
                };

                var result = await Handler.GetByEmployeeCodeAsync(request);

                if (result is { IsSuccess: true })
                    Appointments = result.Data ?? [];
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
        }

        private void ShowEmployee()
        {
            if (!Appointments.Any()) return;

            var appointment = Appointments.FirstOrDefault();
            Employee.Id = appointment?.EmployeeId ?? 0;
            Employee.Name = appointment?.Employee?.Name ?? string.Empty;
        }
        #endregion
    }
}
