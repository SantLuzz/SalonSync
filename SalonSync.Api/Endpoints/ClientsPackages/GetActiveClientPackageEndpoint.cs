using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using SalonSync.Api.Common.Api;
using SalonSync.Core.Handlers.Packages;
using SalonSync.Core.Models.Packages;
using SalonSync.Core.Requests.ClientsPackages;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.ClientsPackages;

public class GetActiveClientPackageEndpoint : IEndpoint
{
        public static void Map(IEndpointRouteBuilder app)
            => app.MapGet("/active/{clientId}", HandlerAsync)
                .WithName("Client Package: Get Active by ClientId")
                .WithDescription("Get a Active package for the Client Id.")
                .WithOrder(7)
                .Produces<Response<ClientPackage?>>();

        private static async Task<IResult> HandlerAsync(
            ClaimsPrincipal user,
            IClientPackageHandler handler,
            long clientId)
        {
            var request = new GetActiveClientPackageRequest()
            {
                UserId = user.Identity?.Name ?? string.Empty,
                ClientId = clientId
            };

            var result = await handler.GetByClientIdAsync(request);
            return result.IsSuccess
                ? TypedResults.Ok(result)
                : TypedResults.NotFound(result);
        }
}