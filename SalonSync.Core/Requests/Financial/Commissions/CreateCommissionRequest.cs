using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Financial.Commissions;

public class CreateCommissionRequest : Request
{
    [Required(ErrorMessage = "Informe o profissíonal")]
    public long EmployeeId { get; set; }
    
    [Required(ErrorMessage = "Informe a data do pagamento da comissão!")]
    public DateTime Date { get; set; }
    
    [Required(ErrorMessage = "Informe o valor da comissão!")]
    public decimal Amount { get; set; }
}