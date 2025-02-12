namespace Parsalud.DataAccess.Models;

public class Section : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Content { get; set; }
    public bool Hidden { get; set; }
}