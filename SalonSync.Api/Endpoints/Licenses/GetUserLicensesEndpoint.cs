using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Licenses;

public class GetUserLicensesEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapGet("/", HandlerAsync)
            .WithName("Get Licenses")
            .Produces<PagedResponse<List<LicenseDto>?>>();


    private  static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ILicenseHandler handler)
    {
        if(!user.IsInRole(Configuration.SuperAdminRole))
            return TypedResults.Unauthorized(); 
        
        var result = await handler.GetAllAsync();
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
            
    }
}