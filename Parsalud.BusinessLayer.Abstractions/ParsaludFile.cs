namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludFile : IDtoBase
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; }
}