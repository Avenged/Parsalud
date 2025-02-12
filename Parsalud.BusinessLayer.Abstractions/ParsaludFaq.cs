namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludFaq
{
    public required Guid Id { get; init; }
    public required string Question { get; init; }
    public required string Answer { get; init; }
    public required bool Hidden { get; init; }
}