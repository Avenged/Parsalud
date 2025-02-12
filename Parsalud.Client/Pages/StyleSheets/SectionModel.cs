using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.StyleSheets;

public class StyleSheetModel
{
    public Guid? Id { get; set; }
    public string? FileName { get; set; }
    public string? Content { get; set; }

    public ManageStyleSheetRequest ToRequest()
    {
        return new ManageStyleSheetRequest
        {
            FileName = FileName ?? "",
            Content = Content ?? "",
        };
    }
}
