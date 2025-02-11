using SalonSync.Core;

namespace SalonSync.Api;

public static class ApiConfiguration
{
    public const string FileFolder = @"wwwroot/uploads/";
    public const string CorsPolicyName = "wasm";
    public const int DefaultTimeByAppointment = 30;
    public static string UrlFiles => $"{Configuration.BackEndUrl}/uploads/";
    public static TimeOnly DefaultStartLunchTime => new TimeOnly(12,0);
    public static TimeOnly DefaultEndLunchTime => new TimeOnly(14,0);
    public static List<DayOfWeek> DefaultAvailableDays =>
    [
        DayOfWeek.Monday,
        DayOfWeek.Tuesday,
        DayOfWeek.Wednesday,
        DayOfWeek.Thursday,
        DayOfWeek.Friday,
        DayOfWeek.Saturday,
    ];

}