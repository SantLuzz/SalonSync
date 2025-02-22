using System.Security.Claims;
using SalonSync.Api.Common.Api;
using SalonSync.Core;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Licenses;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Endpoints.Licenses;

public class UpdateUserLicenseEndpoint : IEndpoint
{
    public static void Map(IEndpointRouteBuilder app)
        => app.MapPut("/update", HandlerAsync)
            .WithName("Update License")
            .Produces<Response<LicenseDto?>>();


    private  static async Task<IResult> HandlerAsync(
        ClaimsPrincipal user,
        ILicenseHandler handler,
        UpdateLicenseRequest request)
    {
        if(!user.IsInRole(Configuration.SuperAdminRole))
            return TypedResults.Unauthorized();
        
        if(request.LicenseEndDate == DateTime.MinValue || request.LicenseEndDate == DateTime.MaxValue)
            return TypedResults.BadRequest(new Response<Company?>(null, 400, "Data da Linceça inválida!"));
        
        var result = await handler.UpdateAsync(request);
        return result.IsSuccess
            ? TypedResults.Ok(result)
            : TypedResults.NotFound(result);
            
    }
}