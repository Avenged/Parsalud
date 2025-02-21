using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Sections;

public class SectionModel : IModelBase<SectionModel, ManageSectionRequest, ParsaludSection>
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

    public SectionModel FromDto(ParsaludSection dto)
    {
        return new SectionModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Code = dto.Code,
            Content = dto.Content,
            StyleSheetId = dto.StyleSheetId,
            Hidden = dto.Hidden,
            Page = dto.Page,
            Param1 = dto.Param1,
            Param2 = dto.Param2,
            Param3 = dto.Param3,
            Param4 = dto.Param4,
            Param5 = dto.Param5,
            Param6 = dto.Param6,
        };  
    }

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
