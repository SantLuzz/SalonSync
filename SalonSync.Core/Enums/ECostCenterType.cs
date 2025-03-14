using System.ComponentModel;

namespace SalonSync.Core.Enums;

public enum ECostCenterType
{
    [Description("Receitas")]
    Income = 1,
    [Description("Despesas")]
    Expense = 2
}
