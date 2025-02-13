using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.BusinessLayer;

namespace Parsalud.Client.Components;

public partial class LatestPostsSection : ComponentBase
{
    private ParsaludPost[] LatestPosts { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await PostService.GetLatestAsync();

        if (response.IsSuccessWithData)
        {
            LatestPosts = response.Data;
        }
    }
}
