using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Parsalud.BusinessLayer.Abstractions;
using System.Globalization;

namespace Parsalud.Client.Components;

public partial class PostContext : ComponentBase
{
    private bool initialized;
    private readonly Dictionary<string, object?> attributes = [];
    private bool found;
    private Dictionary<string, StringValues>? query;
    private const string formato = "MMMM d, yyyy";
    private static readonly CultureInfo cultura = new("es-ES");

    [Parameter]
    [EditorRequired]
    public required string View { get; set; }

    public ParsaludPost? Post { get; set; }

    protected override async Task OnInitializedAsync()
    {
        PostSearchCriteria criteria = GetCurrentCriteria();
        await GetPostByCriteria(criteria);
    }

    private async Task GetPostByCriteria(PostSearchCriteria criteria)
    {
        var response = await Service.GetByCriteriaAsync(criteria);

        if (response.IsSuccessWithData)
        {
            Post = response.Data.Data.FirstOrDefault();
            if (Post is not null)
            {
                found = true;
            }
        }
        else
        {
            found = false;
        }

        attributes.Add("Id", Post?.Id.ToString() ?? "");
        attributes.Add("Title", Post?.Title ?? "");
        attributes.Add("Content", Post?.Content ?? "");
        attributes.Add("Category", Post?.PostCategory ?? "");
        attributes.Add("CreatedAt", Post?.CreatedAt.ToString(formato, cultura) ?? "");
        attributes.Add("Found", found.ToString());
        initialized = true;
    }

    private PostSearchCriteria GetCurrentCriteria()
    {
        PostSearchCriteria criteria = new()
        {
            Page = 0,
            Size = 4,
        };

        Uri uri = new(NM.Uri);
        query = QueryHelpers.ParseQuery(uri.Query);

        foreach (var param in query)
        {
            switch (param.Key)
            {
                case "id":
                    if (Guid.TryParse(param.Value[0], out var id))
                    {
                        criteria.Ids = [id];
                    }
                    break;
                default:
                    break;
            }
        }

        return criteria;
    }
}
