namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludPostCategory
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}