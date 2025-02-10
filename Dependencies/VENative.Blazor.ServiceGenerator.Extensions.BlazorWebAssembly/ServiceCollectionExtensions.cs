using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace VENative.Blazor.ServiceGenerator.Extensions.BlazorWebAssembly;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds the generated service implementations for the Blazor WASM client.
    /// </summary>
    /// <param name="services"></param>
    /// <param name="assembly"></param>
    public static void AddGeneratedServices(this IServiceCollection services, Assembly assembly)
    {
        var interfacesWithAttribute = assembly.GetTypes()
            .Where(t => t.IsInterface && t.GetCustomAttribute<GenerateClientAttribute>() is not null);

        foreach (var interfaceType in interfacesWithAttribute)
        {
            var implementationType = assembly.GetTypes()
                .FirstOrDefault(t => t.GetInterfaces().Contains(interfaceType) && t.IsClass && !t.IsAbstract);

            if (implementationType is not null)
            {
                services.AddTransient(interfaceType, implementationType);
#if DEBUG
                Debug.WriteLine($"Registered '{interfaceType.Name}' with implementation '{implementationType.Name}'");
#endif
            }
            else
            {
#if DEBUG
                Debug.WriteLine($"No implementation found for {interfaceType.Name}");
#endif
            }
        }
    }
}