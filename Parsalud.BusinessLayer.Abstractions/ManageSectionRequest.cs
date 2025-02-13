namespace Parsalud.BusinessLayer.Abstractions;

public class ManageSectionRequest
{
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Content { get; init; }
    public required bool Hidden { get; init; }
    public required Guid? StyleSheetId { get; init; }
}
