using BlazorMonaco;
using BlazorMonaco.Editor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Radzen;

namespace Parsalud.Client.Components;

public partial class MonacoEditor : FormComponent<string>
{
    [Parameter]
    public bool ReadOnly { get; set; }

    [Parameter]
    public RenderFragment? Tools { get; set; }

    [Parameter]
    [EditorRequired]
    public required string Language { get; set; }

    private StandaloneCodeEditor editor = null!;

    private StandaloneEditorConstructionOptions EditorConstructionOptions(StandaloneCodeEditor editor)
    {
        return new StandaloneEditorConstructionOptions
        {
            AutomaticLayout = true,
            Language = Language,
            Value = Value,
            ReadOnly = ReadOnly,
            FormatOnPaste = true,
            FormatOnType = true,
        };
    }

    private async Task Indent()
    {
        var code = await JSRuntime.InvokeAsync<string>("formatCode", await editor.GetValue(), Language);
        await editor.SetValue(code);
        await ChangeInternal(code);
    }

    private async Task OnChange(ChangeEventArgs e)
    {
        await ChangeInternal(e.Value?.ToString());
    }

    private async Task OnInput(KeyboardEvent e)
    {
        await ChangeInternal(await editor.GetValue());
    }

    private async Task ChangeInternal(string? value)
    {
        await ValueChanged.InvokeAsync(value);
        await Change.InvokeAsync(value);

        if (FieldIdentifier.FieldName is not null)
        {
            EditContext?.NotifyFieldChanged(FieldIdentifier);
        }
    }
}
