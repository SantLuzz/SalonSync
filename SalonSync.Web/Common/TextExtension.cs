namespace SalonSync.Web.Common;

public static class TextExtension
{
    public static string ForNumbersOnly(this string? value)
        => string.IsNullOrEmpty(value) ? string.Empty : new string(value.Where(char.IsDigit).ToArray());

}