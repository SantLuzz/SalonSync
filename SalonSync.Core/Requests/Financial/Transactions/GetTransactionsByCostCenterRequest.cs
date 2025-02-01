using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Financial.Transactions;

public class GetTransactionsByCostCenterRequest : PagedRequest
{
    [Required(ErrorMessage = "Informe o cetro de custo!")]
    public long CostCenterId { get; set; }
}