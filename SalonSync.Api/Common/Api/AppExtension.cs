namespace SalonSync.Api.Common.Api
{
    public static class AppExtension
    {
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
}
