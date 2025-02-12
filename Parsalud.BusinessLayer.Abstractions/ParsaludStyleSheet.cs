namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludStyleSheet
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; }
    public required string Content { get; init; }
}