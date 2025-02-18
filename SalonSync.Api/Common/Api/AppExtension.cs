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

}