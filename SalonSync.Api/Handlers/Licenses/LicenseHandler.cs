using Microsoft.EntityFrameworkCore;
using SalonSync.Api.Data;
using SalonSync.Core.DTOs;
using SalonSync.Core.Handlers.Licences;
using SalonSync.Core.Models.Entities;
using SalonSync.Core.Requests.Licenses;
using SalonSync.Core.Responses;

namespace SalonSync.Api.Handlers.Licenses;

public class LicenseHandler(AppDbContext context) : ILicenseHandler
{
    public async Task<bool> CheckAsync(string userId)
    {
        try
        {
            var company = await context
                .Companies
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.UserId == userId);

            if (company is null) return false;
            
            var descriptyDate = Encription.CryptoService.Decrypt(company.LicenseEndDate);

            DateTime dateEndLicense;
            
            try
            {
                dateEndLicense = Convert.ToDateTime(descriptyDate);
            }
            catch
            {
                return false;
            }

            return DateTime.Today <= dateEndLicense;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<PagedResponse<List<LicenseDto>?>> GetAllAsync()
    {
        try
        {
            var query = context
                .Companies
                .AsNoTracking();
            
            var companies =  await query
                .ToListAsync();
            
            var count = await query.CountAsync();

            return new PagedResponse<List<LicenseDto>?>(companies.Select(x => new LicenseDto
            {
                Id = x.Id,
                Name = x.Name,
                UserId = x.UserId,
                CreatedAt = x.CreatedAt,
                LicenseEndDate = !string.IsNullOrWhiteSpace(x.LicenseEndDate)
                    ? Convert.ToDateTime(Encription.CryptoService.Decrypt(x.LicenseEndDate))
                    : null,
            }).ToList(), totalCount: count);
        }
        catch (Exception)
        {
            return new PagedResponse<List<LicenseDto>?>(null, 500, "(SSL001) Não foi possível carregar a lista de licenças das empresas!");
        }
    }

    public async Task<Response<LicenseDto?>> UpdateAsync(UpdateLicenseRequest request)
    {
        try
        {
            var company = await context
                .Companies
                .FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if (company is null) return new Response<LicenseDto?>(null, 404, "Empresa não encontrada!");
            
            company.LicenseEndDate = Encription.CryptoService.Encrypt(request.LicenseEndDate.ToString("dd/MM/yyyy"));
            company.UpdatedAt = DateTime.Now;
            
            context.Companies.Update(company);
            await context.SaveChangesAsync();

            return new Response<LicenseDto?>(new LicenseDto
            {
                Id = company.Id,
                CreatedAt = company.CreatedAt,
                LicenseEndDate = request.LicenseEndDate,
                Name = company.Name,
                UserId = company.UserId,
            }, message: "Licença atualizada com sucesso!");
        }
        catch (Exception)
        {
            return new Response<LicenseDto?>(null, 500, "(SSL002) Não foi possível atualizar a licença da empresa!");
        }
    }
}