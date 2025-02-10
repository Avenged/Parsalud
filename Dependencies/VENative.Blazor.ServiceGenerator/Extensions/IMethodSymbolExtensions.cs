using Microsoft.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace VENative.Blazor.ServiceGenerator.Extensions;

public static class IMethodSymbolExtensions
{
    public static ImmutableArray<AttributeData> GetAttributesByName(this IMethodSymbol methodSymbol, string attributeName)
    {
        return methodSymbol.GetAttributes().Where(a => a.AttributeClass!.Name == attributeName)
            .ToImmutableArray();
    }

    public static ImmutableArray<AttributeData> GetAttributesByNames(this IMethodSymbol methodSymbol, string[] attributeNames)
    {
        return methodSymbol.GetAttributes().Where(a => attributeNames.Contains(a.AttributeClass!.Name))
            .ToImmutableArray();
    }

    public static ImmutableArray<AttributeData> GetAttributesByName(this INamedTypeSymbol namedTypeSymbol, string attributeName)
    {
        return namedTypeSymbol.GetAttributes().Where(a => a.AttributeClass!.Name == attributeName)
            .ToImmutableArray();
    }

    public static ImmutableArray<AttributeData> GetAttributesByNames(this INamedTypeSymbol namedTypeSymbol, string[] attributeNames)
    {
        return namedTypeSymbol.GetAttributes().Where(a => attributeNames.Contains(a.AttributeClass!.Name))
            .ToImmutableArray();
    }
}
