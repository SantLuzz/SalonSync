using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.CostCenters;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Financial;

public class CostCenterHandler(AppDbContext context) : ICostCenterHandler
{
    public async Task<Response<CostCenter?>> CreateAsync(CreateCostCenterRequest request)
    {
        try
        {
            var costCenter = new CostCenter
            {
                UserId = request.UserId,
                Title = request.Title,
                Description = request.Description,
                Type = request.Type,
            };

            await context.CostCenters.AddAsync(costCenter);
            await context.SaveChangesAsync();
            
            return new Response<CostCenter?>(costCenter, 201, "Centro de custo criado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<CostCenter?>(null, 500, "(SSC001) Não foi possível criar o centro de custo!");
        }
    }

    public async Task<Response<CostCenter?>> UpdateAsync(UpdateCostCenterRequest request)
    {
        try
        {
            var cost = await context
                .CostCenters
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (cost is null)
                return new Response<CostCenter?>(null, 404, "Centro de custo não encontrado!");
            
            cost.Title = request.Title;
            cost.Description = request.Description;
            cost.Type = request.Type;
            
            context.CostCenters.Update(cost);
            await context.SaveChangesAsync();
            
            return new Response<CostCenter?>(cost, message: "Centro de custo alterado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<CostCenter?>(null, 500, "(SSC002) Não foi possível alterar o centro de custo!");
        }
    }

    public async Task<Response<CostCenter?>> DeleteAsync(DeleteCostCenterRequest request)
    {
        try
        {
            var cost = await context
                .CostCenters
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            if (cost is null)
                return new Response<CostCenter?>(null, 404, "Centro de custo não encontrado!");
            
            context.Remove(cost);
            await context.SaveChangesAsync();
            
            return new Response<CostCenter?>(cost, message:"Centro de custo excluída com sucesso!");
        }
        catch (Exception)
        {
            return new Response<CostCenter?>(null, 500, "(SSC003) Não foi possível excluir o centro de custo!");
        }
    }

    public async Task<Response<CostCenter?>> GetByIdAsync(GetCostCenterByIdRequest request)
    {
        try
        {
            var cost = await context
                .CostCenters
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            return cost is null
                ? new Response<CostCenter?>(null, 404, "Centro de custo não encontrado!")
                : new Response<CostCenter?>(cost, message: "Centro de custo encontrado com sucesso!"); 
        }
        catch (Exception)
        {
            return new Response<CostCenter?>(null, 500, "(SSC004) Não foi possível recuperar o centro de custo!");
        }
    }

    public async Task<PagedResponse<List<CostCenter>?>> GetAllAsync(GetAllCostCentersRequest request)
    {
        try
        {
            var query = context
                .CostCenters
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId);

            var costs = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<CostCenter>?>(costs, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<CostCenter>?>(null, 500, "(SSC005) Não foi possível recuperar os centros de custos!");
        }
    }

    public async Task<PagedResponse<List<CostCenter>?>> GetAllByTypeAsync(GetAllCostCentersByTypeRequest request)
    {
        try
        {
            var query = context
                .CostCenters
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId && x.Type == request.Type);

            var costs = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<CostCenter>?>(costs, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<CostCenter>?>(null, 500, "(SSC006) Não foi possível recuperar os centros de custos pelo tipo!");
        }
    }
}