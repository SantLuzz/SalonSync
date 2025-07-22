using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Financial.Transactions;

public class UpdateTransactionRequest : Request
{
    public long Id { get; set; }
    [MaxLength(255, ErrorMessage = "A Descrição deve conter no máximo 255 caracteres!")]
    public string Description { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Informe um valor para a movimentação!")]
    [Range(0.01, double.MaxValue, ErrorMessage = "O valor tem que ser maior que zero!")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "O centro de custo é obrigatório!")]
    public long CostId { get; set; }

    [Required(ErrorMessage = "A data de pagamento é obrigatório!")]
    public DateTime? PaidOrReceivedAt { get; set; }
    
    [Required(ErrorMessage = "A forma de pagamento é obrigatório!")]
    public EPaymentType PaymentType { get; set; } = EPaymentType.Cash;
}