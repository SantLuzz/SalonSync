using System.ComponentModel;

namespace SalonSync.Core.Enums;

public enum EAccountStatus
{
    [Description("Todas")]
    All = 0,
    [Description("Abertas")]
    Open = 1,
    [Description("Pagas")]
    Close = 2
}