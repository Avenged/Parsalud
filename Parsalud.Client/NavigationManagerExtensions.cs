using Microsoft.JSInterop;
using Parsalud.Client.Components;
using System.Reflection;

namespace Microsoft.AspNetCore.Components;

public static class NavigationManagerExtensions
{
    public static void NavigateTo(this NavigationManager nm, string uri, Dictionary<string, object?> queryParameters, bool forceLoad = false)
    {
        var uriWithoutQuery = nm.ToAbsoluteUri(uri).GetLeftPart(UriPartial.Path);
        var newUri = nm.GetUriWithQueryParameters(uriWithoutQuery, queryParameters);
        NavigateTo(nm, newUri, forceLoad);
    }

    private static void NavigateTo(NavigationManager nm, string? uri, bool forceLoad = false)
    {
        uri ??= "";

        // Prevent open redirects.
        if (!Uri.IsWellFormedUriString(uri, UriKind.Relative))
        {
            uri = nm.ToBaseRelativePath(uri);
        }

        nm.NavigateTo(uri, forceLoad: forceLoad);
    }

    public async static Task ReplaceUri(this NavigationManager nm, string uri)
    {
        var jsRuntimeField = nm.GetType().GetField("_jsRuntime", BindingFlags.NonPublic | BindingFlags.Instance)!;
        var jsRuntime = (IJSRuntime)jsRuntimeField.GetValue(nm)!;
        await jsRuntime.InvokeVoidAsync("replaceUri", uri);
    }

    public async static Task GoBack(this NavigationManager nm, IJSRuntime jsRuntime)
    {
        if (jsRuntime is IJSInProcessRuntime jsInProcess)
            jsInProcess.InvokeVoid("goBack");
        else
            await jsRuntime.InvokeVoidAsync("goBack");
    }

    public static AbmAction GetAbmAction(this NavigationManager nm)
    {
        string uri = nm.Uri.ToLower();
        var index = nm.Uri.LastIndexOf('?');

        if (index != -1)
        {
            uri = uri[..index];
        }

        if (uri.EndsWith("create"))
            return AbmAction.Create;
        else if (uri.EndsWith("update"))
            return AbmAction.Update;
        else if (uri.EndsWith("view"))
            return AbmAction.View;

        throw new InvalidOperationException("No se reconoció la acción del ABM.");
    }
}