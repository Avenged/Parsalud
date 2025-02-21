namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludPost
{
    public required Guid Id { get; init; }
    public required string Title { get; init; }
    public required string Content { get; init; }
    public required string ImgSrc { get; init; }
    public required bool Hidden { get; init; }
    public required string PostCategory { get; init; }
    public required Guid PostCategoryId { get; init; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}