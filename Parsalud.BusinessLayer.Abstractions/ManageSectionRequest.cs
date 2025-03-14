﻿namespace Parsalud.BusinessLayer.Abstractions;

public class ManageSectionRequest
{
    public required string Name { get; init; }
    public required string Code { get; init; }
    public required string Content { get; init; }
    public required bool Hidden { get; init; }
    public required Guid? StyleSheetId { get; init; }
    public required string? Page { get; init; }
    public required string? Param1 { get; init; }
    public required string? Param2 { get; init; }
    public required string? Param3 { get; init; }
    public required string? Param4 { get; init; }
    public required string? Param5 { get; init; }
    public required string? Param6 { get; init; }
}
