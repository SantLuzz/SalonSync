namespace SalonSync.Core.DTOs;

public class EntityDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string EnumEntity { get; set; } = string.Empty;

    public TEnum GetEnum<TEnum>() where TEnum : struct, Enum
    {
        return Enum.TryParse(EnumEntity, out TEnum result) ? result : throw new InvalidOperationException("Enum inválido.");
    }
}