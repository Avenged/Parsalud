namespace Parsalud.DataAccess.Models;

public class Service : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public List<Faq> Faqs { get; set; } = [];
}