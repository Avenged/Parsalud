using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Posts;

public partial class ManagePosts : ComponentBase
{
    public PostSearchCriteria Criteria { get; } = new();
    public ParsaludPost[]? Posts { get; set; }
    public bool IsSearching { get; set; }

    public async Task Submit()
    {
        Posts = null;
        IsSearching = true;
        await Task.Yield();

        var response = await PostService.GetByCriteriaAsync(Criteria);

        if (response.IsSuccess)
        {
            Posts = response.Data;
        }

        IsSearching = false;
    }

    public void CreateNew()
    {
        NM.NavigateTo("Dashboard/Post/Create");
    }
}
