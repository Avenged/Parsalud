using Microsoft.JSInterop;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheet : BaseAbm<StyleSheetModel, IStyleSheetService, ParsaludStyleSheet, ManageStyleSheetRequest, StyleSheetSearchCriteria>
{
    public ManageStyleSheet() : base("Dashboard/StyleSheets")
    {     
    }

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
}