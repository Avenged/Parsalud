using Microsoft.AspNetCore.Components;
using Parsalud.BusinessLayer.Abstractions;
using System.Globalization;

namespace Parsalud.Client.Components;

public partial class LatestPostsContext : ParsaludComponent
{
    [Parameter]
    public string? View { get; set; }

    [Parameter]
    public string? LoadingView { get; set; }

    private ParsaludPost[]? LatestPosts { get; set; }
    private const string formato = "MMMM d, yyyy";
    private static readonly CultureInfo cultura = new("es-ES");

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
