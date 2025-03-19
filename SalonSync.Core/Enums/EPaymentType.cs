using System.ComponentModel;

namespace SalonSync.Core.Enums;

public enum EPaymentType
{
    [Description("Dinheiro")]
    Cash = 0,

    [Description("Cartão de Crédito")]
    CreditCard = 1,

    [Description("Cartão de Débito")]
    DebitCard = 2,

    [Description("PIX")]
    Pix = 3,

    [Description("Transferência Bancária")]
    BankTransfer = 4,

    [Description("Fiado")]
    Credit = 5
}