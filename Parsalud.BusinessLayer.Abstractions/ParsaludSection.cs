namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludSection
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Content { get; init; }
    public required bool Hidden { get; init; }
}
