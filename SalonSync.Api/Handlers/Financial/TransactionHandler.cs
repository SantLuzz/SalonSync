using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.Transactions;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Financial;

public class TransactionHandler(AppDbContext context) : ITransactionHandler
{
    
    public async Task<Response<Transaction?>> CreateAsync(CreateTransactionRequest request)
    {
        try
        {
            var transaction = new Transaction
            {
                UserId = request.UserId,
                Description = request.Description,
                CreatedAt = DateTime.Now,
                PaidOrReceivedAt = request.PaidOrReceivedAt,
                Amount = request.Amount,
                CostCenterId = request.CostId,
                EntityId = request.EntityId,
                EntityType = request.EntityType
            };
            
            await context.Transactions.AddAsync(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, 201, "Transação cadastrada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Transaction?>(null, 500, "(SST001) Não foi possível cadastrar a transação!");
        }
    }

    public async Task<Response<Transaction?>> UpdateAsync(UpdateTransactionRequest request)
    {
        try
        {
            var transaction = await context
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            if (transaction is null)
                return new Response<Transaction?>(null, 404, "Transação não encontrada!");
            
            transaction.Description = request.Description;
            transaction.CostCenterId = request.CostId;
            transaction.PaidOrReceivedAt = request.PaidOrReceivedAt;
            transaction.Amount = request.Amount;
            
            context.Transactions.Update(transaction);
            await context.SaveChangesAsync();

            return new Response<Transaction?>(transaction, message: "Transação alterada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Transaction?>(null, 500, "(SST002) Não foi possível alterar a transação!");
        }
    }

    public async Task<Response<Transaction?>> DeleteAsync(DeleteTransactionRequest request)
    {
        try
        {
            var transaction = await context
                .Transactions
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);
            
            if (transaction is null)
                return new Response<Transaction?>(null, 404, "Transação não encontrada!");
            
            context.Transactions.Remove(transaction);
            await context.SaveChangesAsync();
            
            return new Response<Transaction?>(transaction, message:"Transação excluída com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Transaction?>(null, 500, "(SST003) Não foi possível excluir a transação!");
        }
    }

    public async Task<Response<Transaction?>> GetByIdAsync(GetTransactionByIdRequest request)
    {
        try
        {
            var transaction = await context
                .Transactions
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && x.UserId == request.UserId);

            return transaction is null
                ? new Response<Transaction?>(null, 404, "Transação não encontrada!")
                : new Response<Transaction?>(transaction, message: "Transação encontrada!");
        }
        catch (Exception)
        {
            return new Response<Transaction?>(null, 500, "(SST004) Não foi possível recuperar a transação!");
        }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByCostCenterAsync(GetTransactionsByCostCenterRequest request)
    {
        try
        {
            var query = context
                .Transactions
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId && x.CostCenterId == request.CostCenterId);
            
            var transactions =  await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Transaction>?>(transactions, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Transaction>?>(null, 500, "(SST005) Não foi possível recuperar as transações pelo centro de custo!");
        }
    }

    public async Task<PagedResponse<List<Transaction>?>> GetByPeriodAsync(GetTransactionsByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();
        }
        catch (Exception)
        {
            return new PagedResponse<List<Transaction>?> (null, 500, "(SST007) Não foi possível determinar a data de ínicio e fim da requisição!");
        }
        
        try
        {
            var query = context
                .Transactions
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId
                    && x.PaidOrReceivedAt >= request.StartDate
                    && x.PaidOrReceivedAt <= request.EndDate)
                .OrderBy(x => x.PaidOrReceivedAt);
            
            var transactions =  await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<Transaction>?>(transactions, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<Transaction>?>(null, 500, "(SST007) Não foi possível recuperar as transações por período!");
        }
    }
}