using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Sections;

public class SectionModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Content { get; set; }
    public Guid? StyleSheetId { get; set; }
    public bool Hidden { get; set; }
    public string? Page { get; set; }
    public string? Param1 { get; set; }
    public string? Param2 { get; set; }
    public string? Param3 { get; set; }
    public string? Param4 { get; set; }
    public string? Param5 { get; set; }
    public string? Param6 { get; set; }

    public ManageSectionRequest ToRequest()
    {
        return new ManageSectionRequest
        {
            Name = Name ?? "",
            Code = Code ?? "",
            Content = Content ?? "",
            StyleSheetId = StyleSheetId,
            Hidden = false,
            Page = Page,
            Param1 = Param1,
            Param2 = Param2,
            Param3 = Param3,
            Param4 = Param4,
            Param5 = Param5,
            Param6 = Param6,
        };
    }
}
