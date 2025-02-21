using Parsalud.Client.Components;
using Parsalud.BusinessLayer.Abstractions;
using Radzen;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePost : BaseAbm<Guid>
{
    public const string MAIN_PATH = "Dashboard/Posts";
    public PostModel Model { get; set; } = new();
    public ParsaludPostCategory[] Categories { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await PostCategoryService.GetByCriteriaAsync(new PostCategorySearchCriteria());

        if (response.IsSuccessWithData)
        {
            Categories = response.Data;
        }
    }

    public async Task Submit()
    {
        if (AbmAction == AbmAction.Create)
        {
            if (!await DS.ConfirmCreationAsync())
            {
                return;
            }

            var response = await Service.CreateAsync(Model.ToRequest());
        }
        else if (AbmAction == AbmAction.Update)
        {
            if (!await DS.ConfirmDeletionAsync())
            {
                return;
            }

            var response = await Service.UpdateAsync(Id, Model.ToRequest());
        }

        NM.NavigateTo(MAIN_PATH);
    }

    public void Discard()
    {
        NM.NavigateTo(MAIN_PATH);
    }
}