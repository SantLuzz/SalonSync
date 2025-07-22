using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Api.ViewModels;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Commissions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Financial;

public class CommissionHandler(AppDbContext context) : ICommissionHandler
{
    public async Task<Response<Commission?>> CreateAsync(CreateCommissionRequest request)
    {
        var commisssion = new Commission
        {
            EmployeeId = request.EmployeeId,
            Date = request.Date,
            Amount = request.Amount,
        };

        await context.Commissions.AddAsync(commisssion);
        await context.SaveChangesAsync();

        return new Response<Commission?>(commisssion, 201,message: "Comissão criada com sucesso!");
    }

    public async Task<Response<Commission?>> DeleteAsync(DeleteCommissionRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<List<UnpaidCommissionDto>?>> GetUnpaidAsync(GetUnpaidCommissionInPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();
        }
        catch (Exception)
        {
            return new PagedResponse<List<UnpaidCommissionDto>?> (null, 500, "(SST003) Não foi possível determinar a data de ínicio e fim da requisição!");
        }

        try
        {
            string sql = $@"SELECT 
                SUM(tr.Amount) AS Total,
                app.EmployeeId,
                ep.Name,
                ep.Commission,
                0.00 AS TotalPayable
            FROM Transactions AS tr
            INNER JOIN Appointments AS app ON tr.EntityId = app.Id
            INNER JOIN Employees AS ep ON app.EmployeeId = ep.Id
            WHERE tr.EntityType = 4 AND ep.Commission > 0
            GROUP BY app.EmployeeId, ep.Name, ep.Commission";

            var transactions = await context
                .Database
                .SqlQueryRaw<UnpaidCommissionDto>(sql)
                .ToListAsync();

            var commissions = await context
                .Commissions
                .AsNoTracking()
                .Where(x => 
                    x.Date >= request.StartDate
                    && x.Date <= request.EndDate)
                .ToListAsync();

            var unpaids = new List<UnpaidCommissionDto>();

            foreach (var transaction in transactions)
            {
                if(commissions.Find(x => x.EmployeeId == transaction.EmployeeId) is null)
                {
                    transaction.TotalPayable = transaction.Commission == 0 ? 0 
                        : transaction.Total * transaction.Commission / 100;
                    unpaids.Add(transaction);
                }
            }

            
            return new PagedResponse<List<UnpaidCommissionDto>?>(unpaids, message: "Comissões carregadas com sucesso!");
        }
        catch (Exception)
        {
            return new PagedResponse<List<UnpaidCommissionDto>?> (null, 500, "(SST004) Não foi possível recuperar os profissíonais para gerar a comissão!");
        }
    }
}