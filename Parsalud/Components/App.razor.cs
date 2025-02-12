using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Parsalud.Components;

public partial class App : ComponentBase
{
    [CascadingParameter]
    private HttpContext HttpContext { get; set; } = default!;

    private static readonly InteractiveServerRenderMode Server = new(prerender: false);
    private static readonly InteractiveWebAssemblyRenderMode Wasm = new(prerender: false);
    private static readonly InteractiveAutoRenderMode Auto = new(prerender: false);
    private static string[] staticSegments = ["/Account", "/NotFound"];
    private static readonly StringComparison comparison = StringComparison.InvariantCultureIgnoreCase;
    private bool useInteractiveIdentityRoutes;
    private bool isDashboard;
    private bool isLanding;
    private IComponentRenderMode? renderModeForPage;

    private IComponentRenderMode? RenderModeForPage
    {
        get => renderModeForPage;
        set
        {
            if (renderModeForPage is not null)
                throw new InvalidOperationException("No se puede reasignar el modo de renderizado una vez ya inicialiado");
            renderModeForPage = value;
        }
    }

    protected override void OnInitialized()
    {
        if (HttpContext.Request.Path.StartsWithSegments("/Dashboard", StringComparison.InvariantCultureIgnoreCase))
        {
            isDashboard = true;
            RenderModeForPage = GetRenderMode();
        }

        if (!HttpContext.Request.Path.StartsWithSegments("/Account", StringComparison.InvariantCultureIgnoreCase))
        {
            isLanding = true;
        }
    }

    /// <summary>
    /// Indica si la solicitud es hacia una ruta relacionada a identity que requiere interactividad.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    private static bool IsInteractiveIdentity(HttpContext context)
        => context.Request.Path.StartsWithSegments("/Account/Login", comparison);

    private static bool IsStatic(HttpContext context)
        => Array.Exists(staticSegments, x => context.Request.Path.StartsWithSegments(x, comparison));

    private IComponentRenderMode? GetRenderMode()
    {
        if (IsInteractiveIdentity(HttpContext))
        {
            useInteractiveIdentityRoutes = true;
            return Server;
        }
        if (IsStatic(HttpContext)) return null;

        // Explicit server rendering mode
        if (HttpContext.Request.Path.StartsWithSegments("/Server", comparison)) return Server;

        // Explicit wasm rendering mode
        if (HttpContext.Request.Path.StartsWithSegments("/Wasm", comparison)) return Wasm;

        // Check if the user has configured a preferred rendering mode
        // if (HttpContext.User.Identity?.IsAuthenticated ?? false)
        // {
        // var principal = HttpContext.User;
        // var preferredRenderingMode = principal.Claims.FirstOrDefault(x => x.Type == AppClaimTypes.PreferredRenderingMode)?.Value;
        // if (!string.IsNullOrWhiteSpace(preferredRenderingMode))
        // {
        //     if (preferredRenderingMode.Equals("Auto", comparison))
        //         return Auto;
        //     else if (preferredRenderingMode.Equals("Server", comparison))
        //         return Server;
        //     else if (preferredRenderingMode.Equals("Wasm", comparison))
        //         return Wasm;
        // }
        // }

        // Default rendering mode
        return Wasm;
    }
}
