using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Employees;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Entities.Employees;

public partial class Edit : ComponentBase
{
    #region Dependencies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IEmployeeHandler Handler { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    #endregion
    
    #region Properties

    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    public bool IsSubmitCode { get; set; }
    public CustomEmployeeViewModel InputModel { get; set; } = new();
    public int EmployeeId { get; set; }
    [Parameter] public string Id { get; set; } = string.Empty;

    #endregion

    #region overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;
        
        EmployeeId = 0;
        try
        {
            EmployeeId = int.Parse(Id);
        }
        catch
        {
           Snackbar.Add("Não foi possível converter os parâmetros!", Severity.Error);
           return;
        }

        try
        {
            var employee = await Handler.GetByIdAsync(new GetEmployeeByIdRequest
            {
                Id = EmployeeId
            });

            if (employee is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível carregar o profissional!", Severity.Error);
                NavigationManager.NavigateTo("/employees");
                return;
            }

            InputModel = new()
            {
                UpdateRequest = new()
                {
                    Id = EmployeeId,
                    Name = employee.Data.Name,
                    Admission = employee.Data.Admission ?? DateTime.MinValue,
                    Email = employee.Data.Email,
                    Phone = employee.Data.Phone,
                    Position = employee.Data.Position,
                    Commission = employee.Data.Commission ?? 0,
                    StartTime = employee.Data.StartTime?.ToTimeSpan(),
                    EndTime = employee.Data.EndTime?.ToTimeSpan(),
                    LunchStart = employee.Data.LunchStart?.ToTimeSpan(),
                    LunchEnd = employee.Data.LunchEnd?.ToTimeSpan(),
                    UserId = employee.Data.UserId
                },
                ImageRequest = new()
                {
                    Id = employee.Data.Id,
                    UserId = employee.Data.UserId,
                    UrlImage = employee.Data.Image
                },
                CodeRequest = new()
                {
                    EmployeeId = employee.Data.Id,
                    UserId = employee.Data.UserId,
                },
                Url = GenerateUrl(employee.Data.UniqueCode)
            };
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Error: {ex.Message}", Severity.Error);
        }
        finally{IsBusy = false;}
    }

    #endregion
    
    #region Methods

    public async Task OnValidSubmit()
    {
        try
        {
            var result = await Handler.UpdateAsync(InputModel.UpdateRequest);

            if (result.IsSuccess)
            {
                Snackbar.Add("Funcionário alterado com sucesso!", Severity.Success);
                NavigationManager.NavigateTo("/employees");
            }
            else
                Snackbar.Add(result.Message, Severity.Error);
        }
        catch (Exception e)
        {
            Snackbar.Add("Não foi possível salvar o funcionário!", Severity.Error);
        }
    }

    public async Task GenerateCodeAsync()
    {
        try
        {
            var result = await Handler.GenerateUniqueCodeAsync(InputModel.CodeRequest);
            if (result is { IsSuccess: false, Data: null })
            {
                Snackbar.Add("Não foi possível gerar a URL do funcionário!", Severity.Error);
                return;
            }

            InputModel.Url = GenerateUrl(result.Data.UniqueCode);
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao gerar url: {ex.Message}", Severity.Error);
        }
    }
    public async Task CopyToClipboardAsync()
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", InputModel.Url);
        Snackbar.Add("Link copiado com sucesso!", Severity.Success);
    }
    
    
    public async Task OnFileUploadAsync(string base64)
    {
        try
        {
            InputModel.ImageRequest.Base64Image = base64;
            
            var result = await Handler.UpdateImageAsync(InputModel.ImageRequest);

            if (!result.IsSuccess)
                Snackbar.Add("Não foi possível alterar a imagem!", Severity.Error);
            else
                Snackbar.Add("Imagem do profissional alterada com sucesso!", Severity.Success);

            InputModel.ImageRequest.UrlImage = result.Data.Image;
            StateHasChanged();
        }
        catch (Exception ex)
        {
            Snackbar.Add($"Erro ao processar a imagem: {ex.Message}", Severity.Error);
        }
    }
    
    #endregion

    #region Private Methods
    private string? GenerateUrl(Guid? code)
    {
        if (code is null) return null;
        
        return WebConfiguration.FrontendUrl + "/scheudules?id=" + code;
    }
    #endregion
}