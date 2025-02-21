namespace Parsalud.DataAccess.Models;

public class Section : DeletableAuditableEntityBase
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Code { get; set; }
    public required string Content { get; set; }
    public Guid? StyleSheetId { get; set; }
    public bool Hidden { get; set; }
    public string? Page { get; set; }
    public string? Param1 { get; set; }
    public string? Param2 { get; set; }
    public string? Param3 { get; set; }
    public string? Param4 { get; set; }
    public string? Param5 { get; set; }
    public string? Param6 { get; set; }

    public virtual StyleSheet? StyleSheet { get; set; }

    public override string ToString()
    {
        return Code + " - " + Name + " - " + Id + " - ";
    }
}