using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace VENative.Blazor.ServiceGenerator.Helpers;

public static class AttributeHelper
{
    public static string GetAttributeInfo(this AttributeData attribute)
    {
        var sb = new StringBuilder();

        var attributeName = attribute.AttributeClass?.Name;
        sb.Append($"[{attributeName}");

        if (attribute.ConstructorArguments.Length > 0 || attribute.NamedArguments.Length > 0)
        {
            sb.Append("(");

            // Constructor arguments (posicionales)
            var constructorArgs = attribute.ConstructorArguments.Select(FormatTypedConstant);
            sb.Append(string.Join(", ", constructorArgs));

            // Named arguments
            if (attribute.NamedArguments.Length > 0)
            {
                if (constructorArgs.Any())
                {
                    sb.Append(", ");
                }
                var namedArgs = attribute.NamedArguments.Select(arg => $"{arg.Key} = {FormatTypedConstant(arg.Value)}");
                sb.Append(string.Join(", ", namedArgs));
            }

            sb.Append(")");
        }

        sb.Append("]");

        return sb.ToString();
    }

    private static string FormatTypedConstant(TypedConstant constant)
    {
        if (constant.IsNull)
        {
            return "null";
        }

        return constant.Kind switch
        {
            TypedConstantKind.Primitive => FormatTypedConstantPrimitive(constant),
            TypedConstantKind.Enum => $"{constant.Type?.Name}.{constant.Value}",
            TypedConstantKind.Type => $"typeof({constant.Value})",
            TypedConstantKind.Array => $"new[] {{ {string.Join(", ", constant.Values.Select(FormatTypedConstant))} }}",
            _ => constant.ToString() ?? "null"
        };
    }

    private static string FormatTypedConstantPrimitive(TypedConstant constant)
    {
        return constant.Type!.Name switch
        {
            "String" => $"\"{constant.Value?.ToString()}\"",
            _ => constant.Value?.ToString() ?? "null"
        };
    }
}
