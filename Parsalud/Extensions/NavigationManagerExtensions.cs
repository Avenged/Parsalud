using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.WebUtilities;

namespace Parsalud.Extensions;

public static class NavigationManagerExtensions
{
    public static Dictionary<string, object> GetQueryParameters(this NavigationManager navManager)
    {
        var uri = navManager.Uri;
        var query = new Uri(uri).Query;

        return QueryHelpers.ParseQuery(query)
            .ToDictionary(kvp => kvp.Key, kvp => (object)(kvp.Value[0]?.ToString() ?? ""));
    }
}
