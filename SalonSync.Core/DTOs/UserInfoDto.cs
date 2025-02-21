namespace SalonSync.Core.DTOs;

public class UserInfoDto
{
    public string? Email { get; set; } 
    public string? Name { get; set; }
    public List<string>? Roles { get; set; } = [];
    public List<ClaimDto>? Claims { get; set; } = [];
}