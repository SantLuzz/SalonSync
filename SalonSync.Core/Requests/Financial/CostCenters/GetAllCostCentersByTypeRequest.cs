using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Financial.CostCenters;

public class GetAllCostCentersByTypeRequest : PagedRequest
{
    [Required(ErrorMessage = "Informe o tipo desejado!")]
    public ECostCenterType Type { get; set; }
}