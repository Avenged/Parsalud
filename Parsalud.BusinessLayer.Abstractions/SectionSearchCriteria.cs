namespace Parsalud.BusinessLayer.Abstractions;

public class SectionSearchCriteria
{
    public string? Name { get; set; }
    public string? Code { get; set; }
    public string? Content { get; set; }
    public SectionKind? SectionKind { get; set; }
}