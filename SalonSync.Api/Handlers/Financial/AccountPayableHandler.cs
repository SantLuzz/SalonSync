using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Common.Extensions;
using SalonSync.Core.Enums;
using SalonSync.Core.Handlers.Financial;
using SalonSync.Core.Models.Financial;
using SalonSync.Core.Requests.Financial.AccountsPayable;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Financial;

public class AccountPayableHandler(AppDbContext context) : IAccountPayableHandler
{
    public async Task<Response<AccountPayable?>> CreateAsync(CreateAccountPayableRequest request)
    {
        try
        {
            if(!request.CreateAccountPayableDetailsList.Any())
                return new Response<AccountPayable?>(null, 400, "Detalhes das contas não foi informado!");

            List<AccountPayable> accountPayables = [];

            foreach (var detail in request.CreateAccountPayableDetailsList)
            {
                accountPayables.Add(new AccountPayable
                {
                    Description = detail.Description,
                    Amount = detail.Amount,
                    CreatedAt = DateTime.Now,
                    DueDate = detail.DueDate ?? DateTime.Today,
                    SupplierName = detail.SupplierName,
                    SupplierDocument = detail.SupplierDocument,
                    UserId = request.UserId,
                });
            }
            
            await context.AccountPayables.AddRangeAsync(accountPayables);
            await context.SaveChangesAsync();
            
            return new Response<AccountPayable?>(accountPayables.FirstOrDefault(), 201, "Contas criadas com sucesso!");
        }
        catch (Exception)
        {
            return new Response<AccountPayable?>(null, 500, "(SSAP001) Não foi possível cadastrar a Conta a pagar!");
        }
    }

    public async Task<Response<AccountPayable?>> UpdateAsync(UpdateAccountPayableRequest request)
    {
        try
        {
            var account = await context
                .AccountPayables
                .FirstOrDefaultAsync(x => x.Id == request.Id
                    && x.UserId == request.UserId);

            if (account is null)
                return new Response<AccountPayable?>(null, 404, "Conta não encontrada!");

            account.Description = request.Description;
            account.Amount = request.Amount;
            account.DueDate = request.DueDate ?? DateTime.Today;
            account.SupplierName = request.SupplierName;
            account.SupplierDocument = request.SupplierDocument;
            
            context.AccountPayables.Update(account);
            await context.SaveChangesAsync();
            
            return new Response<AccountPayable?>(account, 200, "Conta atualizada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<AccountPayable?>(null, 500, "(SSAP002) Não foi possível atualizar a Conta a pagar!");
        }
    }

    public async Task<Response<AccountPayable?>> DeleteAsync(DeleteAccountPayableRequest request)
    {
        try
        {
            var account = await context
                .AccountPayables
                .FirstOrDefaultAsync(x => x.Id == request.Id
                                          && x.UserId == request.UserId);

            if (account is null)
                return new Response<AccountPayable?>(null, 404, "Conta não encontrada!");
            
            
            context.AccountPayables.Remove(account);
            await context.SaveChangesAsync();
            
            return new Response<AccountPayable?>(account, 200, "Conta atualizada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<AccountPayable?>(null, 500, "(SSAP003) Não foi possível excluír a Conta a pagar!");
        }
    }

    public async Task<Response<AccountPayable?>> CloseAsync(CloseAccountPayableRequest request)
    {
        try
        {
            var account = await context
                .AccountPayables
                .FirstOrDefaultAsync(x => x.Id == request.Id
                                          && x.UserId == request.UserId);

            if (account is null)
                return new Response<AccountPayable?>(null, 404, "Conta não encontrada!");

            account.PaymentDate = request.PaymentDate;
            
            context.AccountPayables.Update(account);
            await context.SaveChangesAsync();
            
            return new Response<AccountPayable?>(account, 200, "Conta encerrada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<AccountPayable?>(null, 500, "(SSAP004) Não foi possível encerrar a Conta a pagar!");
        }
    }

    public async Task<Response<AccountPayable?>> GetByIdAsync(GetAccountPayableByIdRequest request)
    {
        try
        {
            var account = await context
                .AccountPayables
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id
                                          && x.UserId == request.UserId);
            
            return account is not null
                ? new Response<AccountPayable?>(account, 200, "Conta encerrada com sucesso!")
                : new Response<AccountPayable?>(null, 404, "Conta não encontrada!");
        }
        catch (Exception)
        {
            return new Response<AccountPayable?>(null, 500, "(SSAP005) Não foi possível recuperar a Conta a pagar!");
        }
    }

    public async Task<PagedResponse<List<AccountPayable>?>> GetByPeriodAsync(GetAccountPayableByPeriodRequest request)
    {
        try
        {
            request.StartDate ??= DateTime.Now.GetFirstDay();
            request.EndDate ??= DateTime.Now.GetLastDay();
        }
        catch (Exception)
        {
            return new PagedResponse<List<AccountPayable>?> (null, 500, "(SSAP007) Não foi possível determinar a data de ínicio e fim da requisição!");
        }
        
        try
        {
            var query = context
                .AccountPayables
                .AsNoTracking()
                .Where(x => x.UserId == request.UserId
                            && x.DueDate >= request.StartDate
                            && x.DueDate <= request.EndDate);
            
            if (request.AccountStatus == EAccountStatus.Open)
            {
                query = query.Where(x => x.PaymentDate == null);
            }
            else if (request.AccountStatus == EAccountStatus.Close)
            {
                query = query.Where(x => x.PaymentDate != null);
            }
            
            query = query
                .OrderBy(x => x.DueDate);
            
            var accountPayable =  await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();
            
            var count = await query.CountAsync();
            
            return new PagedResponse<List<AccountPayable>?>(accountPayable, count, request.PageNumber, request.PageSize);
        }
        catch (Exception)
        {
            return new PagedResponse<List<AccountPayable>?>(null, 500,
                "(SSAP006) Não foi possível recuperar a Conta a pagar!");
        }
    }
}