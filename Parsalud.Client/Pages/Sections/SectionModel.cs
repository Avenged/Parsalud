using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Sections;

public class SectionModel
{
    public Guid? Id { get; set; }
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Content { get; set; }
    public bool Hidden { get; set; }

    public ManageSectionRequest ToRequest()
    {
        return new ManageSectionRequest
        {
            Name = Name ?? "",
            Code = Code ?? "",
            Content = Content ?? "",
            Hidden = false,
        };
    }
}
