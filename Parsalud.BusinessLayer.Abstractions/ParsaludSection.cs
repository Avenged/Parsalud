using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludSection : IDtoBase
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Content { get; init; }
    public required Guid? StyleSheetId { get; init; }
    public required bool Hidden { get; init; }
    public required string? Page { get; init; }
    public required string? Param1 { get; init; }
    public required string? Param2 { get; init; }
    public required string? Param3 { get; init; }
    public required string? Param4 { get; init; }
    public required string? Param5 { get; init; }
    public required string? Param6 { get; init; }

    [JsonIgnore]
    [NotMapped]
    public SectionKind SectionKind { get => string.IsNullOrWhiteSpace(Page) ? SectionKind.Component : SectionKind.Page; }
}
