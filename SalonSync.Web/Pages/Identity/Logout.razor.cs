using Microsoft.AspNetCore.Components;
using MudBlazor;
using SalonSync.Core.Handlers;
using SalonSync.Web.Security;
using SalonSync.Web.Services;

namespace SalonSync.Web.Pages.Identity
{
    public partial class LogoutPage: ComponentBase
    {
        #region Dependencies
        [Inject]
        public ISnackbar Snackbar { get; set; } = null!;

        [Inject] public LogoutService LogoutService { get; set; } = null!;
        #endregion

        #region Overrrides
        protected override async Task OnInitializedAsync()
        {
            await LogoutService.Logout();
            await base.OnInitializedAsync();
        }
        #endregion
    }
}
