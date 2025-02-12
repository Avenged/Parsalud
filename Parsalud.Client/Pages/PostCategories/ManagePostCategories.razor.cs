using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.PostCategories;

public partial class ManagePostCategories : ComponentBase
{
    public const string NEW_ITEM_TEXT = "Nueva categoría";
    public const string CREATE_PATH = "Dashboard/PostCategory/Create";

    public PostCategorySearchCriteria Criteria { get; } = new();
    public ParsaludPostCategory[]? Items { get; set; }
    public bool IsSearching { get; set; }

    public async Task Submit()
    {
        Items = null;
        IsSearching = true;
        await Task.Yield();

        var response = await Service.GetByCriteriaAsync(Criteria);

        if (response.IsSuccess)
        {
            Items = response.Data;
        }

        IsSearching = false;
    }

    public void CreateNew()
    {
        NM.NavigateTo(CREATE_PATH);
    }
}
