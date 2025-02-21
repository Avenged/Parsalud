namespace Parsalud.DataAccess.Models;

public class Post : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string? ImgSrc { get; set; }
    public bool Hidden { get; set; }
    public required Guid PostCategoryId { get; set; }
    public PostCategory PostCategory { get; set; } = null!;
}