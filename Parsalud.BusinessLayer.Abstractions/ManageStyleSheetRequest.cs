namespace Parsalud.BusinessLayer.Abstractions;

public class ManageStyleSheetRequest
{
    public required string FileName { get; init; }
    public required string Content { get; init; }
}