namespace Parsalud.DataAccess.Models;

public abstract class AuditableEntityBase
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid CreatedById { get; set; }
    public ParsaludUser CreatedBy { get; set; } = null!;
    public Guid? UpatedById { get; set; }
    public ParsaludUser? UpatedBy { get; set; }
}