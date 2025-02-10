using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Reflection;

namespace VENative.Blazor.ServiceGenerator.Extensions;

public static class HubMappingExtensions
{
    private static PropertyInfo? _routeTemplateProp;

    /// <summary>
    /// Maps the generated hubs required for consuming services from Blazor WASM.
    /// </summary>
    /// <param name="app"></param>
    /// <param name="assembly"></param>
    public static void MapGeneratedHubs(this IApplicationBuilder app, bool useAuthorization, Assembly assembly)
    {
        var mapHubMethod = typeof(HubEndpointRouteBuilderExtensions)
            .GetMethods(BindingFlags.Static | BindingFlags.Public)
            .First(m => m.Name == "MapHub" && m.GetGenericArguments().Length == 1);

        var types = assembly.GetTypes();
        var hubClasses = types.Where(t => typeof(Hub).IsAssignableFrom(t));

        foreach (var hubClass in hubClasses)
        {
            var hubRoute = GetRouteTemplate(hubClass);
            var genericMapHubMethod = mapHubMethod.MakeGenericMethod(hubClass);

            try
            {
                var builder = (HubEndpointConventionBuilder)genericMapHubMethod.Invoke(null, [app, hubRoute])!;
                if (useAuthorization)
                {
                    builder.RequireAuthorization();
                }
            }
            catch (Exception innerException)
            {
                throw new InvalidOperationException($"An error occurred while configuring the hub '{hubClass.Name}'.", innerException);
            }
        }
    }

    private static string GetRouteTemplate(Type hubClass)
    {
        var attr = hubClass.GetCustomAttribute(typeof(RouteAttribute));
        _routeTemplateProp ??= typeof(RouteAttribute).GetProperty(nameof(RouteAttribute.Template));
        var routeTemplate = (string)_routeTemplateProp!.GetValue(attr)!;
        return routeTemplate;
    }
}