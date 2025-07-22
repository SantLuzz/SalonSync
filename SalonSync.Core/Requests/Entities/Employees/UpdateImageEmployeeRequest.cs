using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Entities.Employees;

public class UpdateImageEmployeeRequest : Request
{
    public long Id { get; set; }
    public string? Base64Image { get; set; } 
    public string? UrlImage { get; set; }
}