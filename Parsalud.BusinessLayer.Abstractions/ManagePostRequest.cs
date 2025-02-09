namespace Parsalud.BusinessLayer.Abstractions;

public class ManagePostRequest
{
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required bool Hidden { get; init; }
    public required Guid PostCategoryId { get; init; }
}