namespace Parsalud.DataAccess.Models;

public abstract class DeletableAuditableEntityBase : AuditableEntityBase
{
    public bool Deleted { get; set; }
}