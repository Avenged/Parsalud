using HtmlAgilityPack;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Caching.Memory;
using Parsalud.BusinessLayer.Abstractions;
using System.Text;

namespace Parsalud.Client.Components;

public partial class DynamicSection : ComponentBase
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? Attributes { get; set; }

    [Parameter]
    [EditorRequired]
    public required string Code { get; set; }

    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public Guid? LiveServerId { get; set; }

    [Parameter]
    public int? I { get; set; }

    [Parameter]
    public string? Param1 { get; set; }

    [Parameter]
    public string? Param2 { get; set; }

    [Parameter]
    public string? Param3 { get; set; }

    [Parameter]
    public string? Param4 { get; set; }

    [Parameter]
    public string? Param5 { get; set; }

    [Parameter]
    public string? Param6 { get; set; }

    [Parameter]
    public int ElementDeep { get; set; }

    private Guid? prevLiveServerId;
    private string? prevCode;
    private int? prevI;
    private string? content;
    private readonly Dictionary<string, Type> componentMappings = new()
    {
        { nameof(CurrentPageTitle), typeof(CurrentPageTitle) },
        { nameof(DynamicSection), typeof(DynamicSection) },
        { nameof(LatestPostsSection), typeof(LatestPostsSection) },
        { nameof(FaqList), typeof(FaqList) }
        // Agrega más componentes aquí según los que pueden llegar desde la base de datos.
    };

    protected override async Task OnParametersSetAsync()
    {
        if (prevI == I &&
            prevCode == Code &&
            prevLiveServerId == LiveServerId)
            return;

        prevI = I;
        prevCode = Code;
        prevLiveServerId = LiveServerId;

        if (LiveServerId.HasValue)
        {
            var ls = MemoryCache.Get<LiveServerInstance>($"LiveServer-{LiveServerId}");
            if (ls is not null)
            {
                StringBuilder sb = new();
                if (!string.IsNullOrWhiteSpace(ls.Css))
                {
                    sb.AppendLine("<style>");
                    sb.AppendLine("@import url('css/bundle.min.css');");
                    sb.AppendLine(ls.Css);
                    sb.AppendLine("</style>");
                }
                if (!string.IsNullOrWhiteSpace(ls.Html))
                {
                    sb.AppendLine(ls.Html);
                }

                var rawContent = sb.ToString();
                content = ReplaceParameters(rawContent);
                return;
            }
        }

        var response = await Service.GetByCodeAsync(
            Code,
            Param1,
            Param2,
            Param3,
            Param4,
            Param5,
            Param6);

        if (response.IsSuccessWithData)
        {
            var rawContent = response.Data.Content;
            content = ReplaceParameters(rawContent);
        }
        else
        {
            NM.NavigateTo("NotFound", replace: true);
        }
    }

    private string ReplaceParameters(string content)
    {
        // Se necesita que esto se procese lo más rápido posible,
        // por eso no se hace de forma más dinámica esta parte.

        // Usar arrays o reflexión podría agregar más overhead, por
        // lo tanto se recomienda hacerlo de la manera más "cruda" posible.

        if (!string.IsNullOrWhiteSpace(Param1))
            content = content.Replace("{Param1}", Param1);

        if (!string.IsNullOrWhiteSpace(Param2))
            content = content.Replace("{Param2}", Param2);

        if (!string.IsNullOrWhiteSpace(Param3))
            content = content.Replace("{Param3}", Param3);

        if (!string.IsNullOrWhiteSpace(Param4))
            content = content.Replace("{Param4}", Param4);

        if (!string.IsNullOrWhiteSpace(Param5))
            content = content.Replace("{Param5}", Param5);

        if (!string.IsNullOrWhiteSpace(Param6))
            content = content.Replace("{Param6}", Param6);

        content = content.Replace("{Uri}", NM.Uri);
        content = content.Replace("{BaseUri}", NM.BaseUri);

        return content;
    }

    private RenderFragment RenderDynamicContent(string html) => builder =>
    {
        var htmlDoc = new HtmlDocument();
        htmlDoc.LoadHtml(html);

        RenderNode(builder, htmlDoc.DocumentNode, 0);
    };

    private void RenderNode(RenderTreeBuilder builder, HtmlNode node, int sequence)
    {
        if (node.NodeType == HtmlNodeType.Document)
        {
            foreach (var childNode in node.ChildNodes)
            {
                RenderNode(builder, childNode, sequence++);
            }
        }

        if (node.NodeType == HtmlNodeType.Text)
        {
            // Renderizar texto directamente
            builder.AddContent(sequence++, node.InnerHtml);
            return;
        }

        if (node.NodeType == HtmlNodeType.Element)
        {
            if (componentMappings.TryGetValue(node.OriginalName, out Type? componentType))
            {
                // Renderizar componente dinámico
                builder.OpenComponent(sequence++, componentType);

                // Agregar atributos como parámetros
                foreach (var attribute in node.Attributes)
                {
                    builder.AddAttribute(sequence++, attribute.OriginalName, attribute.Value);
                }

                if (componentType == typeof(DynamicComponent))
                {
                    builder.AddAttribute(sequence++, nameof(ElementDeep), ElementDeep);
                }

                builder.CloseComponent();
            }
            else
            {
                // Renderizar etiqueta HTML estándar
                builder.OpenElement(sequence++, node.OriginalName);
                ElementDeep++;

                // Agregar atributos
                foreach (var attribute in node.Attributes)
                {
                    var attributeValue = attribute.Value;
                    if (ElementDeep == 1 && (Attributes?.TryGetValue(attribute.Name, out var attr) ?? false))
                    {
                        attributeValue += " " + attr;
                    }
                    builder.AddAttribute(sequence++, attribute.OriginalName, attributeValue);
                }

                // Renderizar hijos recursivamente
                foreach (var childNode in node.ChildNodes)
                {
                    RenderNode(builder, childNode, sequence++);
                }

                builder.CloseElement();
            }
        }
    }
}