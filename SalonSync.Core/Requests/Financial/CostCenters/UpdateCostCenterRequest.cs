using System.ComponentModel.DataAnnotations;
using SalonSync.Core.Enums;

namespace SalonSync.Core.Requests.Financial.CostCenters;

public class UpdateCostCenterRequest : Request
{
    public long Id { get; set; }
    
    [Required(ErrorMessage = "O Título é obrigatório!")]
    [MinLength(3, ErrorMessage = "O Título deve conter no mínimo 3 caracteres!")]
    [MaxLength(100, ErrorMessage = "O Título deve conter no máximo 100 caracteres!")]
    public string Title { get; set; } = string.Empty;

    [MaxLength(255, ErrorMessage = "A Descrição deve conter no máximo 255 caracteres!")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "O Tipo de movimentação é obrigatório!")]
    public ECostCenterType Type { get; set; } = ECostCenterType.Income;
}