using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Parsalud.BusinessLayer.Abstractions;
using System.Globalization;

namespace Parsalud.Client.Components;

public partial class CategoriesContext : ComponentBase
{
    [Parameter]
    public string? View { get; set; }

    [Parameter]
    public string? NotFoundView { get; set; }

    [Parameter]
    public string? LoadingView { get; set; }

    public ParsaludPostCategory[]? Categories { get; set; }

    private Dictionary<string, StringValues>? query;
    private const string formato = "MMMM d, yyyy";
    private static readonly CultureInfo cultura = new("es-ES");

    protected override async Task OnInitializedAsync()
    {
        var response = await PostCategoryService.GetByCriteriaAsync(new PostCategorySearchCriteria());
        if (response.IsSuccessWithData)
        {
            Categories = response.Data;
        }
        else
        {
            Categories = [];
        }
    }

    private bool IsChecked(Guid id)
    {
        Uri uri = new(NM.Uri);
        query ??= QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("c", out var values))
        {
            var culture = StringComparison.InvariantCultureIgnoreCase;
            return values.Any(x => x?.Equals(id.ToString(), culture) ?? false);
        }
        return false;
    }
}
