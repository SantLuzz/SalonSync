using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Handlers.Licences;

namespace SalonSync.Api.Endpoints.Licenses;

public class CheckCompanyLicenseEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/check-license", HandlerAsync)
            .WithName("CheckCompanyLicense")
            .Produces<bool>();


    private  static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ILicenseHandler handler)
    {
        if (user.IsInRole(Configuration.SuperAdminRole))
            return TypedResults.Ok(true);
        
        var result = await handler.CheckAsync(user.Identity?.Name ?? string.Empty);
        return TypedResults.Ok(result);
            
    }
}