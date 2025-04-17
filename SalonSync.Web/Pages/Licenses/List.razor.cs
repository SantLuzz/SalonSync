using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Requests.Licenses;
using SalonSync.Web.Components.Dialogs;

namespace SalonSync.Web.Pages.Licenses;

public partial class List : ComponentBase
{
        #region Properties
        public bool IsBusy { get; set; }
        public List<LicenseDto> Licenses { get; set; } = [];
        public string SearchTerm { get; set; } = string.Empty;
        #endregion

        #region services
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IDialogService DialogService { get; set; } = null!;

        [Inject]
        public ILicenseHandler Handler { get; set; } = null!;
        #endregion

        #region overrides
        protected override async Task OnInitializedAsync()
        {
            IsBusy = true;
            try
            {
                var result = await Handler.GetAllAsync();
                if (result.IsSuccess)
                    Licenses = result.Data ?? [];
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally { IsBusy = false; }
        }
        #endregion

        #region Methods

        public async void OnEditLicenseClickedAsync(string userId)
        {
            IsBusy = true;
            try
            {
                
                var dataSelecionada = await OpenDialogAsync();
                
                if(dataSelecionada is null)
                {
                    Snackbar.Add("Nenhuma data selecionada!", Severity.Error);
                    return;
                }

                var request = new UpdateLicenseRequest
                {
                    UserId = userId,
                    LicenseEndDate = dataSelecionada.Value
                };
                
                var result = await Handler.UpdateAsync(request);
                if (!result.IsSuccess || result.Data is null)
                {
                    Snackbar.Add("Não foi possível atualizar a data da licença!", severity: Severity.Error);
                    return;
                }
                
                Licenses.RemoveAll(x => x.UserId == userId);
                Licenses.Add(result.Data);
                
            }
            catch (Exception ex)
            {
                Snackbar.Add(ex.Message, Severity.Error);
            }
            finally {
                IsBusy = false;
                StateHasChanged();
            }
        }

        
        private async Task<DateTime?> OpenDialogAsync()
        {
            DateTime? returnValue;
            string valor;
            
            var options = new DialogOptions
            {
                CloseOnEscapeKey = true,
            };
            var dialogReference = await DialogService.ShowAsync<DatePickerDialog>("Informe a data de expiração da licença.", options);
            StateHasChanged();
            var dialogResult = await dialogReference.Result;
            if (dialogResult.Canceled)
                returnValue = null;
            else
                returnValue = Convert.ToDateTime(dialogResult.Data);
            
            StateHasChanged();

            return returnValue;
        }
        
        public Func<LicenseDto, bool> Filter => license =>
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
                return true;
        
            if (license.Id.ToString().Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;
        
            if(license.Name.Contains(SearchTerm, StringComparison.OrdinalIgnoreCase))
                return true;
        
            if (license.LicenseEndDate is not null && license.LicenseEndDate == Convert.ToDateTime(SearchTerm))
                return true;
        
        
            return false;
        };
        #endregion
}