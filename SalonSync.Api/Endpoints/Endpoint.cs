using SalonSync.Api.Common.Api;
using SalonSync.Api.Endpoints.Appointments;
using SalonSync.Api.Endpoints.ClientsPackages;
using SalonSync.Api.Endpoints.Entities.Clients;
using SalonSync.Api.Endpoints.Entities.Employees;
using SalonSync.Api.Endpoints.Financial.CostCenters;
using SalonSync.Api.Endpoints.Identity;
using SalonSync.Api.Endpoints.Packages;
using SalonSync.Api.Endpoints.PackagesServices;
using SalonSync.Api.Endpoints.Services;
using SalonSync.Api.Models;

namespace SalonSync.Api.Endpoints
{
    public static class Endpoint
    {
        public static void MapEndpoints(this WebApplication app)
        {
            var endpoint = app
                .MapGroup("");

            endpoint.MapGroup("/")
                .WithTags("Health Check")
                .MapGet("/", () => new { Message = "OK" });
            
            //mapeando os endpoints do identity
            endpoint.MapGroup("v1/identity")
                .WithTags("Identity")
                .MapIdentityApi<User>();

            endpoint.MapGroup("v1/identity")
                .WithTags("Identity")
                .MapEndpoints<LogoutEndpoint>()
                .MapEndpoints<GetRolesEndpoint>();

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
                .MapEndpoints<GetAllClientsByTypeEndpoint>();

            endpoint.MapGroup("v1/employees")
                .WithTags("Employees")
                .RequireAuthorization()
                .MapEndpoints<CreateEmployeeEndpoint>()
                .MapEndpoints<UpdateEmployeeEndpoint>()
                .MapEndpoints<UpdateImageEmployeeEndpoint>()
                .MapEndpoints<DeleteEmployeeEndpoint>()
                .MapEndpoints<GetEmployeeByIdEndpoint>()
                .MapEndpoints<GetAllEmployeesEndpoint>();
            
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
                .MapEndpoints<GetAllPackagesEndpoint>();
            
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
                .MapEndpoints<GetAllPackagesByClientIdEndpoint>();

            endpoint.MapGroup("v1/appointments")
                .WithTags("Appointments")
                .RequireAuthorization()
                .MapEndpoints<OpenAppointmentByEmployeeEndpoint>();
        }

        private static IEndpointRouteBuilder MapEndpoints<TEndpoint> (this IEndpointRouteBuilder app) where TEndpoint : IEndpoint
        {
            TEndpoint.Map(app);
            return app;
        }
    }
}
