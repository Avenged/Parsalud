using Microsoft.AspNetCore.Components;

namespace Parsalud.Client.Components;

public class ParsaludComponent : ComponentBase
{
    /// <summary>
    /// Specifies additional custom attributes that will be rendered by the component.
    /// </summary>
    /// <value>The attributes.</value>
    [Parameter(CaptureUnmatchedValues = true)]
    public IReadOnlyDictionary<string, object>? Attributes { get; set; }

    /// <summary>
    /// Gets the final CSS class rendered by the component. Combines it with a <c>class</c> custom attribute.
    /// </summary>
    protected string GetCssClass()
    {
        if (Attributes != null && Attributes.TryGetValue("class", out var @class) && !string.IsNullOrEmpty(Convert.ToString(@class)))
        {
            return $"{GetComponentCssClass()} {@class}";
        }

        return GetComponentCssClass();
    }

    /// <summary>
    /// Gets the component CSS class.
    /// </summary>
    protected virtual string GetComponentCssClass()
    {
        return "";
    }
}
