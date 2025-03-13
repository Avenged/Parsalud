namespace Parsalud.DataAccess.Models;

public class Faq : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Question { get; set; }
    public required string Answer { get; set; }
    public bool Hidden { get; set; }
    public Guid? ServiceId { get; set; }
    public Service? Service { get; set; }
}