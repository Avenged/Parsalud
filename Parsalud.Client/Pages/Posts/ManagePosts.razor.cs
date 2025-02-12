using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePosts : ComponentBase
{
    public const string NEW_ITEM_TEXT = "Nuevo artículo";
    public const string CREATE_PATH = "Dashboard/Post/Create";

    public PostSearchCriteria Criteria { get; } = new();
    public ParsaludPost[]? Items { get; set; }
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
