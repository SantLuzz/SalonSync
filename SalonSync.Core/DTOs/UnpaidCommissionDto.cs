namespace SalonSync.Core.DTOs;

public class UnpaidCommissionDto
{
    public long EmployeeId { get; set; }
    public string Name { get; set; } = String.Empty;
    public decimal Commission { get; set; }
    public decimal Total { get; set; }
    public decimal TotalPayable { get; set; }
}