using SalonSync.Core;

namespace SalonSync.Api;

public static class ApiConfiguration
{
    public const string FileFolder = @"wwwroot/uploads/";
    public const string CorsPolicyName = "wasm";
    public const int DefaultTimeByAppointment = 30;
    public static string UrlFiles => $"{Configuration.BackEndUrl}/uploads/";
    
}