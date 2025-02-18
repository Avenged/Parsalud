using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Components;

public partial class LatestPostsSection : ParsaludComponent
{
    [Parameter]
    [EditorRequired]
    public required string PostSectionView { get; set; }

    [Parameter]
    [EditorRequired]
    public required string PostView { get; set; }

    private ParsaludPost[] LatestPosts { get; set; } = [];

    protected override async Task OnInitializedAsync()
    {
        var response = await PostService.GetLatestAsync();

        if (response.IsSuccessWithData)
        {
            LatestPosts = response.Data;
        }
    }

    protected override string GetComponentCssClass()
    {
        return "latest-posts";
    }
}
