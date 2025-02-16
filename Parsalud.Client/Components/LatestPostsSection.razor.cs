using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Components;

public partial class LatestPostsSection : ParsaludComponent
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

    protected override string GetComponentCssClass()
    {
        return "latest-posts";
    }
}
