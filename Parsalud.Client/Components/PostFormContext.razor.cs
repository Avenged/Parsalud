using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using Radzen;

namespace Parsalud.Client.Components;

public partial class PostFormContext : ComponentBase
{
    [Parameter]
    public string? View { get; set; }

    private Dictionary<string, StringValues>? query;

    private string? GetInputTextValue()
    {
        Uri uri = new(NM.Uri);
        query ??= QueryHelpers.ParseQuery(uri.Query);

        if (query.TryGetValue("s", out var value))
        {
            return value;
        }
        return null;
    }
}