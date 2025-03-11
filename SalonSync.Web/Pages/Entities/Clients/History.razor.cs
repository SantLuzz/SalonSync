using Microsoft.AspNetCore.Components;

namespace SalonSync.Web.Pages.Entities.Clients;

public partial class History : ComponentBase
{
    #region Properties
    [Parameter] public string Id { get; set; } = string.Empty;
    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    public int ClientId { get; set; }

    #endregion
}