using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Entities;

public class CompanyHandler(AppDbContext context) : ICompanyHandler
{
    public async Task<Response<Company?>> CreateAsync(CreateCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            if(company is not null)
                return new Response<Company?>(null, 400, "Já existe uma empresa vinculada a esse e-mail!");

            company = new Company
            {
                Name = request.Name,
                Document = request.Document,
                Phone = request.Phone,
                UserId = request.UserId,
                Code = Guid.NewGuid(),
            };
            
            await context.Companies.AddAsync(company);
            await context.SaveChangesAsync();
            
            return new Response<Company?>(company, 201, "Empresa cadastrada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Company?>(null, 500, "(SSCP001) Não foi possível cadastrar a empresa!");
        }
    }
    
    public async Task<Response<Company?>> UpdateAsync(UpdateCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            if(company is null)
                return new Response<Company?>(null, 404, "Empresa não encontrada!");

            company.Name = request.Name;
            company.Document = request.Document;
            company.Phone = request.Phone;
            
            context.Companies.Update(company);
            await context.SaveChangesAsync();
            
            return new Response<Company?>(company, message: "Empresa alterada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Company?>(null, 500, "(SSCP002) Não foi possível alterar a empresa!");
        }
    }
    
    public async Task<Response<Company?>> DeleteAsync(DeleteCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            if(company is null)
                return new Response<Company?>(null, 404, "Empresa não encontrada!");
            
            context.Companies.Remove(company);
            await context.SaveChangesAsync();
            
            return new Response<Company?>(company, message: "Empresa removida com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Company?>(null, 500, "(SSCP003) Não foi possível remover a empresa!");
        }
    }

    public async Task<Response<Company?>> GetAsync(GetCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            return company is null
                ? new Response<Company?>(null, 404, "Empresa não encontrada!")
                : new Response<Company?>(company, message: "Empresa encontrada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Company?>(null, 500, "(SSCP004) Não foi possível recuperar a empresa!");
        }
    }

    public async Task<Response<Company?>> GenerateCodeAsync(GenerateCodeByCompanyRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);
            
            if(company is null)
                return new Response<Company?>(null, 404, "Empresa não encontrada!");
            
            company.Code = Guid.NewGuid();
            
            context.Companies.Update(company);
            await context.SaveChangesAsync();
            
            return new Response<Company?>(company, message: "Código da empresa gerado com sucesso!");
        }
        catch (Exception)
        {
            return new Response<Company?>(null, 500, "(SSCP004) Não foi possível gerar o código a empresa!");
        }
    }
}