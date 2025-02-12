namespace Parsalud.DataAccess.Models;

public class StyleSheet : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string FileName { get; set; }
    public required string Content { get; set; }
}