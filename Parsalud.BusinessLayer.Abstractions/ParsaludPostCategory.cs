﻿namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludPostCategory : IDtoBase
{
    public required Guid Id { get; init; }
    public required string Name { get; init; }
    public required int PostsCount { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; init; }
}