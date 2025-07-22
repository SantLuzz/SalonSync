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
    
    [Description("Contas a Pagar")]
    AccountPayable = 3,
    
    [Description("Agendamentos")]
    Appointments = 4,
    
    [Description("Comissão")]
    Comission = 5,
}