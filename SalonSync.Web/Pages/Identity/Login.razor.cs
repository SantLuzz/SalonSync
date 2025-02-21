using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers;
using SalonSync.Core.Requests.Account;
using SalonSync.Web.Security;
using SalonSync.Web.Services;

namespace SalonSync.Web.Pages.Identity
{
    public partial class LoginPage : ComponentBase
    {
        #region Dependencies
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject]
        public IAccountHandler Handler { get; set; } = null!;

        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;

        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;

        [Inject] public LicenseService LicenseService { get; set; } = null!;
        [Inject] public LogoutService LogoutService { get; set; } = null!;
        #endregion

        #region properties
        public bool IsBusy { get; set; } = false;
        public LoginRequest InputModel { get; set; } = new();
        
        public InputType passwordInputType = InputType.Password;
        public string passwordIcon = Icons.Material.Filled.VisibilityOff;
        #endregion

        #region Overrrides
        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            //merge pattern
            if (user.Identity is { IsAuthenticated: true })
                NavigationManager.NavigateTo("/");
        }


        #endregion

        #region Methods
        public async Task OnValidSubmitAsync()
        {
            IsBusy = true;
            try
            {
                var result = await Handler.LoginAsync(InputModel);

                if (result.IsSuccess)
                {
                    if (!await LicenseService.ValidateLicenseAsync())
                    {
                        Snackbar.Add("Não foi possível validar o login!", Severity.Error);
                        await LogoutService.Logout();
                        return;
                    }
                    
                    await AuthenticationStateProvider.GetAuthenticationStateAsync();
                    AuthenticationStateProvider.NotifyAuthenticationStateChanged();
                    NavigationManager.NavigateTo("/");
                }
                else
                    Snackbar.Add(result?.Message ?? string.Empty, Severity.Error);
            }
            catch (Exception e)
            {
                Snackbar.Add(e.Message, Severity.Error);
            }
            finally { IsBusy = false; }
        }
        
        public void TogglePasswordVisibility()
        {
            if (passwordInputType == InputType.Password)
            {
                passwordInputType = InputType.Text;
                passwordIcon = Icons.Material.Filled.Visibility;
            }
            else
            {
                passwordInputType = InputType.Password;
                passwordIcon = Icons.Material.Filled.VisibilityOff;
            }
        }

        #endregion
    }
}
