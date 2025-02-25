namespace SalonSync.Web.Common;

public class Masks
{
    public static string ApplyPhoneMask(string value)
    {
        if (string.IsNullOrEmpty(value)) return value;

        if (value.Length <= 10) // Formato (XX) XXXX-XXXX
            return value.Length switch
            {
                > 6 => $"({value[..2]}) {value[2..6]}-{value[6..]}",
                > 2 => $"({value[..2]}) {value[2..]}",
                _ => value
            };  

        // Formato (XX) XXXXX-XXXX
        return value.Length switch
        {
            > 7 => $"({value[..2]}) {value[2..7]}-{value[7..]}",
            > 2 => $"({value[..2]}) {value[2..]}",
            _ => value
        };
    }

    public static string ApplyDocumentMask(string value)
    {
        if (value.Length <= 11) // CPF
        {
            return value.Length switch
            {
                > 9 => $"{value[..3]}.{value[3..6]}.{value[6..9]}-{value[9..]}",
                > 6 => $"{value[..3]}.{value[3..6]}.{value[6..]}",
                > 3 => $"{value[..3]}.{value[3..]}",
                _ => value
            };
        }
        else // CNPJ
        {
            return value.Length switch
            {
                > 12 => $"{value[..2]}.{value[2..5]}.{value[5..8]}/{value[8..12]}-{value[12..]}",
                > 8 => $"{value[..2]}.{value[2..5]}.{value[5..8]}/{value[8..]}",
                > 5 => $"{value[..2]}.{value[2..5]}.{value[5..]}",
                > 2 => $"{value[..2]}.{value[2..]}",
                _ => value
            };
        }
    }
}