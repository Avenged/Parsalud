using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.StyleSheets;

public class StyleSheetModel : IModelBase<StyleSheetModel, ManageStyleSheetRequest, ParsaludStyleSheet>
{
    public StyleSheetModel()
    {
    }

    public Guid? Id { get; set; }
    public string? FileName { get; set; }
    public string? Content { get; set; }

    public StyleSheetModel FromDto(ParsaludStyleSheet dto)
    {
        return new StyleSheetModel
        {
            Id = dto.Id,
            FileName = dto.FileName,
            Content = dto.Content,
        };
    }

    public ManageStyleSheetRequest ToRequest()
    {
        return new ManageStyleSheetRequest
        {
            FileName = FileName ?? "",
            Content = Content ?? "",
        };
    }
}
