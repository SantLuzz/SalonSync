using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;
using SalonSync.Core.Handlers.Entities;
using SalonSync.Core.Requests.Entities.Companies;
using SalonSync.Web.ViewModels;

namespace SalonSync.Web.Pages.Entities.Companies;

public partial class Company : ComponentBase
{
    #region Dependecies

    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ICompanyHandler Handler { get; set; } = null!;
    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IJSRuntime JsRuntime { get; set; } = null!;
    #endregion
    
    #region Properties
    
    public CompanyViewModel InputModel { get; set; } = new();
    public bool IsBusy { get; set; }
    public bool IsSubmit { get; set; }
    public bool IsSubmitCode { get; set; }
    #endregion

    #region Overrides

    protected override async Task OnInitializedAsync()
    {
        IsBusy = true;

        try
        {
            var company =  await Handler.GetAsync(new GetCompanyRequest());

            if (company is {IsSuccess:true, Data: null})
            {
                Snackbar.Add($"Empresa não Encontrada!", Severity.Error);
                NavigationManager.NavigateTo("/");
                return;
            }

            InputModel = new()
            {
                UpdateRequest = new ()
                {
                    Id = company.Data.Id,
                    Name = company.Data.Name,
                    Document = company.Data.Document,
                    Phone = company.Data.Phone,
                },
                CodeRequest = new(),
                Url = GenerateUrl(company.Data.Code)
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

    public async Task OnValidSubmitAsync()
    {
        IsSubmit = true;
        try
        {
            var result = await Handler.UpdateAsync(InputModel.UpdateRequest);

            if (!result.IsSuccess || result.Data is null)
            {
                Snackbar.Add("Não foi possível atualizar os dados da empresa!", Severity.Error);
                return;
            }

            InputModel.UpdateRequest = new()
            {
                Id = result.Data.Id,
                Document = result.Data.Document,
                Name = result.Data.Name,
                Phone = result.Data.Phone,
                UserId = result.Data.UserId,
            };
            
            Snackbar.Add("Empresa alterada com suecesso!", Severity.Success);
            
            StateHasChanged();
        }
        catch (Exception e)
        {
            Snackbar.Add($"Error: {e.Message}", Severity.Error);
        }
        finally{IsSubmit = false;}
    }   
    
    public async Task GenerateCodeAsync()
    {
        IsSubmitCode = true;
        try
        {
            var result = await Handler.GenerateCodeAsync(new GenerateCodeByCompanyRequest());

            if (!result.IsSuccess || result.Data is null)
            {
                Snackbar.Add("Não foi possível gerar a URL da empresa!", Severity.Error);
                return;
            }

            InputModel.Url = GenerateUrl(result.Data.Code);
            Snackbar.Add("Código gerado com sucesso!", Severity.Success);
            StateHasChanged();
        }
        catch (Exception e)
        {
            Snackbar.Add($"Error: {e.Message}", Severity.Error);
        }
        finally{IsSubmitCode = false;}
    }

    public async Task CopyToClipboardAsync()
    {
        await JsRuntime.InvokeVoidAsync("navigator.clipboard.writeText", InputModel.Url);
        Snackbar.Add("Link copiado com sucesso!", Severity.Success);
    }
    
    private string GenerateUrl(Guid code)
    {
        return WebConfiguration.FrontendUrl + "/chat?id=" + code;
    }
    
    #endregion
}