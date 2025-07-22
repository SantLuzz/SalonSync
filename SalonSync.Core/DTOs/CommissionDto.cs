namespace SalonSync.Core.DTOs;

public class CommissionDto
{
    public long Id { get; set; }
    public long EmployeeId { get; set; }
    public string EmployeeName { get; set; } = String.Empty;
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
}