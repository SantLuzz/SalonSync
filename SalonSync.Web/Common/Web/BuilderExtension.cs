using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SalonSync.Core.Handlers;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Handlers.Services;
using SalonSync.Web.Handlers;
using SalonSync.Web.Handlers.Appointments;
using SalonSync.Web.Handlers.Entities;
using SalonSync.Web.Handlers.Financial;
using SalonSync.Web.Handlers.Licenses;
using SalonSync.Web.Handlers.Packages;
using SalonSync.Web.Handlers.Services;
using SalonSync.Web.Security;
using SalonSync.Web.Services;

namespace SalonSync.Web.Common.Web;

public static class BuilderExtension
{
    public static void AddConfiguration(this WebAssemblyHostBuilder builder)
    {
        WebConfiguration.BackendUrl = builder.Configuration.GetValue<string>("BackendUrl") 
                                   ?? string.Empty;
        WebConfiguration.FrontendUrl = builder.Configuration.GetValue<string>("FrontendUrl") 
                                       ?? string.Empty;
    }

    public static void AddRoots(this WebAssemblyHostBuilder builder)
    {
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");
    }

    public static void AddSecurity(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<CookieHandler>();
        builder.Services.AddAuthorizationCore();
        builder.Services.AddScoped<AuthenticationStateProvider, CookieAuthenticationStateProvider>();
        builder.Services.AddScoped(x => 
            (ICookieAuthenticationStateProvider)x.GetRequiredService<AuthenticationStateProvider>());
    }

    public static void AddExternalComponets(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddMudServices();
        builder.Services.AddHttpClient(WebConfiguration.HttpClientName, opt =>
        {
            opt.BaseAddress = new Uri(WebConfiguration.BackendUrl);
        }).AddHttpMessageHandler<CookieHandler>();
    }
    
    public static void AddServices(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddScoped<LogoutService>();
        builder.Services.AddScoped<LicenseService>();
        builder.Services.AddTransient<PaymentService>();
        builder.Services.AddTransient<ControlAppointmentsClientService>();
        builder.Services.AddTransient<IAccountHandler, AccountHandler>();
        builder.Services.AddTransient<ICompanyHandler, CompanyHandler>();
        builder.Services.AddTransient<ILicenseHandler, LicenseHandler>();
        builder.Services.AddTransient<IEmployeeHandler, EmployeeHandler>();
        builder.Services.AddTransient<IClientHandler, ClientHandler>();
        builder.Services.AddTransient<IServiceHandler, ServiceHandler>();
        builder.Services.AddTransient<IPackageHandler, PackageHandler>();
        builder.Services.AddTransient<IPackageServiceHandler, PackageServiceHandler>();
        builder.Services.AddTransient<IClientPackageHandler, ClientPackageHandler>();
        builder.Services.AddTransient<ICostCenterHandler, CostCenterHandler>();
        builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
        builder.Services.AddTransient<IAccountPayableHandler, AccountPayableHandler>();
        builder.Services.AddTransient<IAppointmentHandler, AppointmentHandler>();
    }

    public static void AddLocalization(this WebAssemblyHostBuilder builder)
    {
        builder.Services.AddLocalization();
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = new System.Globalization.CultureInfo("pt-BR");
        System.Globalization.CultureInfo.DefaultThreadCurrentUICulture = new System.Globalization.CultureInfo("pt-BR");
    }
}