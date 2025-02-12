using Parsalud.Client.Components;

namespace Parsalud.Client.Pages.PostCategories;

public partial class ManagePostCategory : BaseAbm<Guid>
{
    public const string MAIN_PATH = "Dashboard/PostCategories";
    public PostCategoryModel Model { get; set; } = new();

    public async Task Submit()
    {
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