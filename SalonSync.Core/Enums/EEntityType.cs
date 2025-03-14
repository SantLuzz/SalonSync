using System.ComponentModel;

namespace SalonSync.Core.Enums;

public enum EEntityType
{
    [Description("Transação Direta no Caixa")]
    DirectTransaction = 0, 

    [Description("Serviços")]
    Services = 1,

    [Description("Pacotes")]
    Packages = 2,
}