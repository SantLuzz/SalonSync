using System.ComponentModel.DataAnnotations;

namespace SalonSync.Core.Requests
{
    public abstract class Request
    {
        [EmailAddress(ErrorMessage = "Usuário logado inválido!")]
        [Required(ErrorMessage = "Informe o usuário para prosseguir!")]
        public string UserId { get; set; } = string.Empty;
    }
}
