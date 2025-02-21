using Microsoft.AspNetCore.Identity;
using SalonSync.Api.Data;
using SalonSync.Api.Models;
using SalonSync.Core;
using SalonSync.Core.Models.Entities;

namespace SalonSync.Api.Common.Api;

public static class AppExtension
{
    public static void LoadConfiguration(this WebApplication app)
    {
        var smtp = new ApiConfiguration.SmtpConfiguration();
        app.Configuration.GetSection("Smtp").Bind(smtp);
        ApiConfiguration.Smtp = smtp;
    }
        
    public static void ConfigureDevEnvironment(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
        app.MapSwagger()
            .RequireAuthorization();
    }

    public static void UseFiles(this WebApplication app)
    {
        app.UseStaticFiles();
    }
        
    public static void UseSecurity(this WebApplication app)
    {
        app.UseAuthentication();
        app.UseAuthorization();
    }


    public static async void CreateSuperAdminAndRoles(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole<long>>>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var context = services.GetRequiredService<AppDbContext>();

        string[] roleNames =
        [
            Configuration.SuperAdminRole, Configuration.CompanyUserRole,
            Configuration.EmployeeUserRole
        ];

        foreach (var roleName in roleNames)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                await roleManager.CreateAsync(new IdentityRole<long>(roleName));
            }
        }

        // Criar o superadmin
        string email = "contato@salonsyncapp.com";
        string password = ApiConfiguration.SuperAdminPass;
        var superAdmin = await userManager.FindByEmailAsync(email);

        if (superAdmin == null)
        {
            superAdmin = new User
            {
                UserName = email,
                Email = email
            };

            var createUserResult = await userManager.CreateAsync(superAdmin, password);
            if (createUserResult.Succeeded)
            {
                await userManager.AddToRoleAsync(superAdmin, Configuration.SuperAdminRole);
            }
        }

        var company = new Company
        {
            Name = "Admin",
            Document = "",
            Code = new Guid("41c8b055-a3c5-4907-86fc-103bc5f053da"),
            UserId = email
        };
        
        await context.Companies.AddAsync(company);
        await context.SaveChangesAsync();
    }
}