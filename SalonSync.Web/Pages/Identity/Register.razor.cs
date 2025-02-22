using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Account;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Web.Common;
using SalonSync.Web.Security;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Identity
{
    public partial class RegisterPage : ComponentBase
    {
        #region Dependencies
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;
        [Inject]
        public IAccountHandler Handler { get; set; } = null!;
        
        [Inject]
        public ICompanyHandler CompanyHandler { get; set; } = null!;
        
        [Inject]
        public NavigationManager NavigationManager { get; set; } = null!;
        [Inject]
        public ICookieAuthenticationStateProvider AuthenticationStateProvider { get; set; } = null!;
        #endregion

        #region Properties

        protected bool IsBusy { get; set; } = false;
        public RegisterViewModel InputModel { get; set; } = new();
        
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
                var result = await Handler.RegisterAsync(InputModel.RegisterRequest);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result?.Message ?? string.Empty, Severity.Error);
                    return;                    
                }
                
                var auth = await AuthenticateUser(InputModel.RegisterRequest.Email, InputModel.RegisterRequest.Password);

                if (auth is false)
                {
                    Snackbar.Add("Usuário não autenticado!", Severity.Error);
                    return;
                }
                
                InputModel.CompanyRequest.Phone = InputModel.CompanyRequest.Phone?.ForNumbersOnly();
                InputModel.CompanyRequest.UserId = InputModel.RegisterRequest.Email;
                
                //Autorizar o usuário, acho que redirecionar ele para a tela inicial é uma boa,
                
                var companyResult = await CompanyHandler.CreateAsync(InputModel.CompanyRequest);
                if(!companyResult.IsSuccess)
                    Snackbar.Add(companyResult?.Message ?? string.Empty, Severity.Error);
                
                Snackbar.Add(result?.Message ?? string.Empty, Severity.Success);
                NavigationManager.NavigateTo("/");
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
        
        private async Task<bool> AuthenticateUser(string email, string password)
        {
            var result = await Handler.LoginAsync(new LoginRequest
            {
                Email = email,
                Password = password
            });
            
            if (result.IsSuccess)
            {
                await AuthenticationStateProvider.GetAuthenticationStateAsync();
                AuthenticationStateProvider.NotifyAuthenticationStateChanged();
            }
            
            return result.IsSuccess;
        }
        #endregion
    }
}
