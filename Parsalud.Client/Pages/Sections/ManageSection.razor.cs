using Microsoft.JSInterop;
using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.Sections;

public partial class ManageSection : BaseAbm<Guid>
{
    public const string MAIN_PATH = "Dashboard/Sections";
    public SectionModel Model { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        if (Id == Guid.Empty)
        {
            return;
        }

        var response = await Service.GetByIdAsync(Id);

        if (response.IsSuccessWithData)
        {
            Model = new SectionModel()
            {
                Id = response.Data.Id,
                Code = response.Data.Code,
                Name = response.Data.Name,
                Content = response.Data.Content,
                Hidden = response.Data.Hidden,
            };
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("loadLazyCss", "/app.css", "app-style-css");
        }
    }

    public async Task Submit()
    {
        if (ReadOnly)
        {
            return;
        }

        if (AbmAction == AbmAction.Create)
        {
            var response = await Service.CreateAsync(Model.ToRequest());
        }
        else if (AbmAction == AbmAction.Update)
        {
            var response = await Service.UpdateAsync(Id, Model.ToRequest());
        }

        NM.NavigateTo(MAIN_PATH);
    }

    public void Discard()
    {
        NM.NavigateTo(MAIN_PATH);
    }
}