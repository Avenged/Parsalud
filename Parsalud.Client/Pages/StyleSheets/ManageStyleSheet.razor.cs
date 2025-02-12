using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.StyleSheets;

public partial class ManageStyleSheet : BaseAbm<Guid>
{
    public const string MAIN_PATH = "Dashboard/StyleSheets";
    public StyleSheetModel Model { get; set; } = new();

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