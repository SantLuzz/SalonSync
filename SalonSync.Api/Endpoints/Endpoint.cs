using Microsoft.AspNetCore.Identity;
using SalonSync.Api.Common.Api;
using SalonSync.Api.Endpoints.Appointments;
using SalonSync.Api.Endpoints.ClientsPackages;
using SalonSync.Api.Endpoints.Configs;
using SalonSync.Api.Endpoints.Entities.Clients;
using SalonSync.Api.Endpoints.Entities.Companies;
using SalonSync.Api.Endpoints.Entities.Employees;
using SalonSync.Api.Endpoints.Financial.CostCenters;
using SalonSync.Api.Endpoints.Identity;
using SalonSync.Api.Endpoints.Licenses;
using SalonSync.Api.Endpoints.Packages;
using SalonSync.Api.Endpoints.PackagesServices;
using SalonSync.Api.Endpoints.Schedules;
using SalonSync.Api.Endpoints.Services;
using SalonSync.Api.Models;
using SalonSync.Api.Services;
using SalonSync.Core.Requests.ClientsPackages;

namespace SalonSync.Api.Endpoints;

public static class Endpoint
{
    public static void MapEndpoints(this WebApplication app)
    {
        var endpoint = app
            .MapGroup("");

        endpoint.MapGroup("/")
            .WithTags("Health Check")
            .MapGet("/", () => new { Message = "OK" });
        
        endpoint.MapGet("/email", (EmailService emailService) =>
        {
            var to = "teste@teste.com";
            var name = "Teste de email";
            var subject = "Teste de E-mail";
            var body = "Este é um teste de envio de e-mail.";

            var result = emailService.Send(name, to, subject, body);
            
            return result
                ? Results.Ok(new { Message = "E-mail enviado com sucesso!" })
                : Results.Problem("Falha ao enviar o e-mail.");
        });

        
        //mapeando os endpoints do identity
        endpoint.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapIdentityApi<User>();

        endpoint.MapGroup("v1/identity")
            .WithTags("Identity")
            .MapEndpoints<RegisterUserEndpoint>()
            .MapEndpoints<LogoutEndpoint>()
            .MapEndpoints<GetRolesEndpoint>()
            .MapEndpoints<LoginUserEndpoint>()
            .MapEndpoints<GetUserInfoEndpoint>();

        endpoint.MapGroup("v1/configs")
            .WithTags("Configs")
            .RequireAuthorization()
            .MapEndpoints<UpdateSystemConfigEndpoint>()
            .MapEndpoints<GetSystemConfigEndpoint>();

        endpoint.MapGroup("v1/clients")
            .WithTags("Clients")
            .RequireAuthorization()
            .MapEndpoints<CreateClientEndpoint>()
            .MapEndpoints<UpdateClientEndpoint>()
            .MapEndpoints<UpdateImageClientEndpoint>()
            .MapEndpoints<UpdateLastedProcedreClientEndpoint>()
            .MapEndpoints<DeleteClientEndpoint>()
            .MapEndpoints<GetClientByIdEndpoint>()
            .MapEndpoints<GetAllClientsEndpoint>()
            .MapEndpoints<GetAllClientsByTypeEndpoint>()
            .MapEndpoints<GetBirthdaysOfWeekEndpoint>();

        endpoint.MapGroup("v1/companies")
            .WithTags("Companies")
            .RequireAuthorization()
            .MapEndpoints<CreateCompanyEndpoint>()
            .MapEndpoints<UpdateCompanyEndpoint>()
            .MapEndpoints<GenerateCodeByCompanyEndpoint>()
            .MapEndpoints<DeleteCompanyEndpoint>()
            .MapEndpoints<GetCompanyEndpoint>();

        endpoint.MapGroup("v1/employees")
            .WithTags("Employees")
            .RequireAuthorization()
            .MapEndpoints<CreateEmployeeEndpoint>()
            .MapEndpoints<UpdateEmployeeEndpoint>()
            .MapEndpoints<UpdateImageEmployeeEndpoint>()
            .MapEndpoints<DeleteEmployeeEndpoint>()
            .MapEndpoints<GetEmployeeByIdEndpoint>()
            .MapEndpoints<GetAllEmployeesEndpoint>()
            .MapEndpoints<GenerateUniqueEmployeeEndpoint>();
            
        endpoint.MapGroup("v1/costs-centers")
            .WithTags("Costs")
            .RequireAuthorization()
            .MapEndpoints<CreateCostCenterEndpoint>()
            .MapEndpoints<DeleteCostCenterEndpoint>()
            .MapEndpoints<UpdateCostCenterEndpoint>()
            .MapEndpoints<GetAllCostCenterEndpoint>()
            .MapEndpoints<GetAllCostCentersByTypeEndpoint>()
            .MapEndpoints<GetCostCenterByIdEndpoint>();

        endpoint.MapGroup("v1/services")
            .WithTags("Services")
            .RequireAuthorization()
            .MapEndpoints<CreateServiceEndpoint>()
            .MapEndpoints<UpdateServiceEndpoint>()
            .MapEndpoints<DeleteServiceEndpoint>()
            .MapEndpoints<GetServiceByIdEndpoint>()
            .MapEndpoints<GetAllServicesEndpoint>();

        endpoint.MapGroup("v1/packages")
            .WithTags("Packages")
            .RequireAuthorization()
            .MapEndpoints<CreatePackageEndpoint>()
            .MapEndpoints<UpdatePackageEndpoint>()
            .MapEndpoints<DeletePackageEndpoint>()
            .MapEndpoints<GetPackageByIdEndpoint>()
            .MapEndpoints<GetAllPackagesEndpoint>()
            .MapEndpoints<GetPackageServiceDtoByIdEndpoint>();

        endpoint.MapGroup("v1/packages-services")
            .WithTags("Packages-Services")
            .RequireAuthorization()
            .MapEndpoints<CreatePackageServiceEndpoint>()
            .MapEndpoints<UpdatePackageServiceEndpoint>()
            .MapEndpoints<DeletePackageServiceEndpoint>()
            .MapEndpoints<GetPackageServiceByIdEndpoint>();

        endpoint.MapGroup("v1/packages-clients")
            .WithTags("Packages-Clients")
            .RequireAuthorization()
            .MapEndpoints<CreateClientPackageEndpoint>()
            .MapEndpoints<DeleteClientPackageEndpoint>()
            .MapEndpoints<InactiveClientPackageEndpoint>()
            .MapEndpoints<GetClientPackagesByIdEndpoint>()
            .MapEndpoints<GetActiveClientPackageEndpoint>()
            .MapEndpoints<GetAllPackagesByClientIdEndpoint>()
            .MapEndpoints<InactiveClientIsActivePackageEndpoint>();

        endpoint.MapGroup("v1/appointments")
            .WithTags("Appointments")
            .RequireAuthorization()
            .MapEndpoints<OpenAppointmentByEmployeeEndpoint>()
            .MapEndpoints<GenerateScheduleWithEmployeeEndpoint>()
            .MapEndpoints<GetAvailableDateEndpoint>()
            .MapEndpoints<GetEmployeesWithOpenScheduleEndpoint>()
            .MapEndpoints<GetAvailableHoursByEmployeeIdEndpoint>()
            .MapEndpoints<GetAppointmentByEmployeeEndpoint>()
            .MapEndpoints<FinishScheudullingEndpoint>()
            .MapEndpoints<CancelScheudullingEndpoint>();

        endpoint.MapGroup("v1/schedules")
            .WithTags("Schedules")
            .MapEndpoints<GetAppointmentByEmployeeCodeEndpoint>()
            .MapEndpoints<GetAvailableDateByCompanyEndpoint>()
            .MapEndpoints<GenerateScheduleWithEmployeeByCompanyCodeEndpoint>()
            .MapEndpoints<GetAvailableHoursByEmployeeIdAndCompanyCodeEndpoint>()
            .MapEndpoints<GetEmployeesWithOpenScheduleByCompanyCodeEndpoint>();

        endpoint.MapGroup("v1/licenses")
            .WithTags("Licenses")
            .RequireAuthorization()
            .MapEndpoints<CheckCompanyLicenseEndpoint>();

        endpoint.MapGroup("v1/licenses")
            .WithTags("Licenses")
            .RequireAuthorization()
            .MapEndpoints<GetUserLicensesEndpoint>()
            .MapEndpoints<UpdateUserLicenseEndpoint>();

    }

    private static IEndpointRouteBuilder MapEndpoints<TEndpoint> (this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
    {
        TEndpoint.Map(app);
        return app;
    }
}