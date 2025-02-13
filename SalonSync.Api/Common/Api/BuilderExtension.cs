using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Api.Handlers;
using SalonSync.Api.Handlers.Appointments;
using SalonSync.Api.Handlers.Entities;
using SalonSync.Api.Handlers.Financial;
using SalonSync.Api.Handlers.Packages;
using SalonSync.Api.Handlers.Services;
using SalonSync.Api.Models;
using SalonSync.Core;
using SalonSync.Core.Handlers;
using SalonSync.Core.Handlers.Appointments;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Handlers.Services;

namespace SalonSync.Api.Common.Api
{
    public static class BuilderExtension
    {
        public static void AddConfiguration(this WebApplicationBuilder builder)
        {
            Configuration.ConnectionString = builder.Configuration
                .GetConnectionString("DefaultConnection") ?? string.Empty;

            Configuration.BackEndUrl = builder.Configuration.GetValue<string>("BackEndUrl") ?? string.Empty;
            Configuration.FrontEndUrl = builder.Configuration.GetValue<string>("FrontEndUrl") ?? string.Empty;
        }

        public static void AddDocumentation(this WebApplicationBuilder builder) 
        {
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(x =>
            {
                x.CustomSchemaIds(n => n.FullName);
            });
        }

        public static void AddSecurity(this WebApplicationBuilder builder) 
        {
            //adicionando autenticação via cookies
            builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
                .AddIdentityCookies();

            //adicionando autorização
            builder.Services.AddAuthorization();
        }

        public static void AddDataContexts(this WebApplicationBuilder builder) 
        {
            builder.Services.AddDbContext<AppDbContext>(x
                => x.UseSqlServer(Configuration.ConnectionString));

            //Adicionando o suporte do Identity e configurando a relação do mesmo com o EF
            builder.Services.AddIdentityCore<User>() //adiciona um usuário customizado
                .AddRoles<IdentityRole<long>>() //adiciona roles personalizados
                .AddEntityFrameworkStores<AppDbContext>() //adiciona o contexto
                .AddApiEndpoints(); //adiciona suporte para API (gera as apis para login, logout, etc.)
        }

        public static void AddServices(this WebApplicationBuilder builder)
        {
            builder.Services.AddTransient<ISystemConfigHandler, SystemConfigHandler>();
            builder.Services.AddTransient<ICostCenterHandler, CostCenterHandler>();
            builder.Services.AddTransient<ITransactionHandler, TransactionHandler>();
            builder.Services.AddTransient<IClientHandler, ClientHandler>();
            builder.Services.AddTransient<IEmployeeHandler, EmployeeHandler>();
            builder.Services.AddTransient<ICompanyHandler, CompanyHandler>();
            builder.Services.AddTransient<IServiceHandler, ServiceHandler>();
            builder.Services.AddTransient<IPackageHandler, PackageHandler>();
            builder.Services.AddTransient<IPackageServiceHandler, PackageServiceHandler>();
            builder.Services.AddTransient<IClientPackageHandler, ClientPackageHandler>();
            builder.Services.AddTransient<IAppointmentHandler, AppointmentHandler>();
        }

        public static void AddCrossOrigin(this WebApplicationBuilder builder)
        {
            builder.Services.AddCors(options => options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy => policy
                .WithOrigins([
                    Configuration.BackEndUrl,
                    Configuration.FrontEndUrl
                    ])
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials()
                ));
        }
    }
}
