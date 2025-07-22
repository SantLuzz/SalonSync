using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests.Financial.AccountsPayable;

public class UpdateAccountPayableRequest : Request
{
    public long Id { get; set; }
    [MaxLength(255, ErrorMessage = "A descrição deve conter no máximo 255 caracteres!")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Informe o nome do fornecedor!")]
    [MinLength(3, ErrorMessage = "O nome do fornecedor deve conter ao menos 3 caracteres!")]
    [MaxLength(255, ErrorMessage = "O nome do fornecedor deve conter no máximo 255 caracteres!")]
    public string SupplierName { get; set; } = string.Empty;
    
    [MaxLength(255, ErrorMessage = "O documento do fornecedor deve conter no máximo 20 caracteres!")]
    public string? SupplierDocument { get; set; }
    
    [Required(ErrorMessage = "Informe um valor para a movimentação!")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor tem que ser maior que zero!")]
    public decimal Amount { get; set; }
    
    [Required(ErrorMessage = "Informe a data de Vencimento!")]
    public DateTime? DueDate { get; set; }
}