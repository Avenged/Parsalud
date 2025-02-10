using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace VENative.Blazor.ServiceGenerator.Extensions;

public static class ImmutableArrayExtensions
{
    public static IEnumerable<IParameterSymbol> ExcludeCancellationToken(this ImmutableArray<IParameterSymbol> arr)
    {
        return arr.Where(p => p.Type.ToDisplayString() != "System.Threading.CancellationToken");
    }

    public static bool HasCancellationToken(this ImmutableArray<IParameterSymbol> arr)
    {
        return arr.Any(p => p.Type.ToDisplayString() == "System.Threading.CancellationToken");
    }
}