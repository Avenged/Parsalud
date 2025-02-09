namespace Parsalud.DataAccess.Models;

public class PostCategory : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public virtual List<Post> Posts { get; set; } = [];
}