using SalonSync.Core.Enums;

namespace SalonSync.Core.DTOs;

public class ClientPackageDto
{
    public long Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int RemainingSessions  { get; set; }
    public long ClientId { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public long PackageId { get; set; }
    public string PackageName { get; set; } = string.Empty;
    public EPackageStatus Status { get; set; } = EPackageStatus.Active;
}