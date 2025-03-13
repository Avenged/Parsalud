namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludFaq : IDtoBase
{
    public required Guid Id { get; init; }
    public required string Question { get; init; }
    public required string Answer { get; init; }
    public required bool Hidden { get; init; }
    public required Guid? ServiceId { get; init; }
    public required DateTime CreatedAt { get; init; }
}