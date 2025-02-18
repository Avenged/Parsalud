using Microsoft.JSInterop;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;
using Radzen.Blazor;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheet : BaseAbm<Guid>
{
    public const string MAIN_PATH = "Dashboard/StyleSheets";
    public StyleSheetModel Model { get; set; } = new();
    public RadzenTemplateForm<StyleSheetModel> Form { get; set; } = null!;

    private DotNetObjectReference<ManageStyleSheet> reference = null!;

    protected override async Task OnInitializedAsync()
    {
        if (Id == Guid.Empty)
        {
            return;
        }

        var response = await Service.GetByIdAsync(Id);

        if (response.IsSuccessWithData)
        {
            Model = new StyleSheetModel()
            {
                Id = response.Data.Id,
                FileName = response.Data.FileName,
                Content = response.Data.Content,
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        reference = DotNetObjectReference.Create(this);

        if (firstRender)
        {
            await JS.InvokeVoidAsync("setDotnetReference", reference);
        }
    }

    [JSInvokable]
    public async Task Save()
    {
        await ManualSubmit();
    }

    public async Task ManualSubmit()
    {
        if (!Form.IsValid)
        {
            Form.EditContext.Validate();
            return;
        }

        var response = await GeneralSubmit(keep: true);

        if (response?.IsSuccess ?? false)
        {
            NS.Notify(Radzen.NotificationSeverity.Success, "Cambios guardados");
        }
        else
        {
            NS.Notify(Radzen.NotificationSeverity.Warning, summary: "No pudimos guardar los cambios", detail: response?.Message);
        }

        if (AbmAction == AbmAction.Create && (response?.IsSuccess ?? false))
        {
            NM.NavigateTo(MAIN_PATH);
        }
    }

    public async Task Submit()
    {
        await GeneralSubmit(keep: false);
    }

    private async Task<BusinessResponse?> GeneralSubmit(bool keep)
    {
        if (ReadOnly)
        {
            return null;
        }

        BusinessResponse? response;
        if (AbmAction == AbmAction.Create)
        {
            response = await Service.CreateAsync(Model.ToRequest());
        }
        else if (AbmAction == AbmAction.Update)
        {
            response = await Service.UpdateAsync(Id, Model.ToRequest());
        }
        else
        {
            throw new NotImplementedException();
        }

        if (!keep)
        {
            NM.NavigateTo(MAIN_PATH);
        }

        return response;
    }

    public void Discard()
    {
        NM.NavigateTo(MAIN_PATH);
    }
}