using Microsoft.JSInterop;
using Parsalud.Client.Components;
using Parsalud.BusinessLayer.Abstractions;
using Parsalud.Client.Pages.StyleSheets;
using System.Text;
using Radzen.Blazor.Rendering;

namespace Parsalud.Client.Pages.Sections;

public partial class ManageSection : BaseAbm<SectionModel, ISectionService, ParsaludSection, ManageSectionRequest, SectionSearchCriteria>
{
    public ManageSection() : base("Dashboard/Sections")
    {
    }

    public List<ParsaludStyleSheet> StyleSheets { get; set; } = [];
    public ParsaludStyleSheet? StyleSheet { get; set; }
    public StyleSheetModel? StyleSheetModel { get; set; }
    public StyleSheetModel CreateStyleSheetModel { get; set; } = new();
    public Popup? Popup { get; set; }
    public ButtonWithTooltip? Button { get; set; }
    public string? ErrorMsg { get; set; }
    public bool CreatingStyleSheet { get; set; }
    public bool ParametrosRutaSelected { get; set; }
    public Guid LiveServerId { get; set; }

    private readonly string editorHtmlId = "ed_" + Guid.NewGuid().ToString();
    private readonly string editorCssId = "ed_" + Guid.NewGuid().ToString();
    private int iframei;
    private DotNetObjectReference<ManageSection> reference = null!;

    public string? DesignerContent
    {
        get
        {
            StringBuilder sb = new();
            if (StyleSheetModel is not null)
            {
                sb.AppendLine("<style>");
                sb.AppendLine("@import url('css/bundle.min.css');");
                sb.AppendLine(StyleSheetModel.Content);
                sb.AppendLine("</style>");
            }
            sb.AppendLine(designerContent);
            return sb.ToString();
        }
        set
        {
            designerContent = value;
        }
    }

    private string? designerContent;

    protected override async Task OnInitializedAsync()
    {
        LiveServerId = Guid.NewGuid();

        await FetchStyleSheetsAsync();

        if (Id == Guid.Empty)
        {
            return;
        }

        await FetchResourceAsync();
        await Service.UpdateLiveServerAsync(new LiveServerInstance
        {
            Id = LiveServerId,
            Css = StyleSheetModel?.Content,
            Html = Model.Content,
        });
    }

    private async Task FetchResourceAsync()
    {
        var response = await Service.GetByIdAsync(Id);

        if (response.IsSuccessWithData)
        {
            DesignerContent = response.Data.Content;
            var data = response.Data;

            Model = new SectionModel()
            {
                Id = data.Id,
                Code = data.Code,
                Name = data.Name,
                Content = data.Content,
                Hidden = data.Hidden,
                Page = data.Page,
                Param1 = data.Param1,
                Param2 = data.Param2,
                Param3 = data.Param3,
                Param4 = data.Param4,
                Param5 = data.Param5,
                Param6 = data.Param6,
                StyleSheetId = response.Data.StyleSheetId,
            };

            if (response.Data.StyleSheetId is not null)
            {
                StyleSheet = StyleSheets.FirstOrDefault(x => x.Id == response.Data.StyleSheetId);
                StyleSheetModel = new()
                {
                    Id = StyleSheet?.Id,
                    FileName = StyleSheet?.FileName,
                    Content = StyleSheet?.Content,
                };
            }
        }
    }

    private async Task FetchStyleSheetsAsync()
    {
        var response = await StyleSheetService.GetByCriteriaAsync(new StyleSheetSearchCriteria());

        if (response.IsSuccessWithData)
        {
            StyleSheets = [..response.Data.Data];
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        reference = DotNetObjectReference.Create(this);

        if (firstRender)
        {
            await JS.InvokeVoidAsync("setDotnetReference", reference);
            await JS.InvokeVoidAsync("loadLazyCss", $"/{AppConstants.MAIN_CSS_FILENAME}", "app-style-css");
        }
    }

    [JSInvokable]
    public async Task Save()
    {
        await ManualSubmit();
    }

    protected override async Task<BusinessResponse<ParsaludSection>?> GeneralSubmit(bool keep)
    {
        if (ReadOnly)
        {
            return null;
        }

        var styleSheetSaved = await SubmitStyleSheetAsync();

        if (styleSheetSaved is not null && !styleSheetSaved.Value)
        {
            ErrorMsg = "No se pudo guardar los cambios de la hoja de estilo";
            return null;
        }

        return await base.GeneralSubmit(keep);
    }

    private async Task<bool?> SubmitStyleSheetAsync()
    {
        if (StyleSheetModel is null || !StyleSheetModel.Id.HasValue)
        {
            return null;
        }

        var response = await StyleSheetService.UpdateAsync(StyleSheetModel.Id.Value, StyleSheetModel.ToRequest());
        return response.IsSuccess;
    }

    private void StyleSheetChange(ParsaludStyleSheet? parsaludStyleSheet)
    {
        Model.StyleSheetId = parsaludStyleSheet?.Id;
        StyleSheet = parsaludStyleSheet;

        if (parsaludStyleSheet is not null)
        {
            StyleSheetModel = new StyleSheetModel()
            {
                Id = parsaludStyleSheet?.Id,
                Content = parsaludStyleSheet?.Content,
                FileName = parsaludStyleSheet?.FileName,
            };
        }
        else
        {
            StyleSheetModel = null;
        }
    }

    private async Task CreateNewStyleSheet()
    {
        CreateStyleSheetModel = new();
        await Popup!.ToggleAsync(Button!.Button.Element);
    }

    private async Task CreateStyleSheet()
    {
        if (string.IsNullOrWhiteSpace(CreateStyleSheetModel.FileName))
        {
            return;
        }

        CreatingStyleSheet = true;
        await Task.Yield();

        var response = await StyleSheetService.CreateAsync(CreateStyleSheetModel.ToRequest());

        if (response.IsSuccessWithData)
        {
            StyleSheets.Add(response.Data);
            StyleSheetChange(response.Data);
        }
        else
        {
            NS.Notify(Radzen.NotificationSeverity.Warning, summary: "No se pudo crear la hoja de estilo", detail: response.Message);
        }

        CreateStyleSheetModel = new();
        CreatingStyleSheet = false;
        await Popup!.CloseAsync();
    }

    private void StyleChanged(string? content)
    {
        StyleSheetModel!.Content = content;
        Service.UpdateLiveServerAsync(new LiveServerInstance
        {
            Id = LiveServerId,
            Css = StyleSheetModel?.Content,
            Html = Model.Content,
        });
        iframei++;
    }

    private void SourceChange(string? value)
    {
        Model.Content = value;
        DesignerContent = value;
        Service.UpdateLiveServerAsync(new LiveServerInstance
        {
            Id = LiveServerId,
            Css = StyleSheetModel?.Content,
            Html = Model.Content,
        });
        iframei++;
    }

    private void CodeChanged()
    {
        Service.UpdateLiveServerAsync(new LiveServerInstance
        {
            Id = LiveServerId,
            Css = StyleSheetModel?.Content,
            Html = Model.Content,
        });
        iframei++;
    }
}