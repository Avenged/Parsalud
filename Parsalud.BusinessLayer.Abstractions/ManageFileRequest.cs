namespace Parsalud.BusinessLayer.Abstractions;

public class ManageFileRequest
{
    public required Guid Id { get; init; }
    public required string FileName { get; init; }
}
