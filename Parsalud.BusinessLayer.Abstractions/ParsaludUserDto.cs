namespace Parsalud.BusinessLayer.Abstractions;

public class ParsaludUserDto
{
    public required Guid Id { get; init; }
    public required string UserName { get; init; }
    public required bool LockoutEnabled { get; init; }
}
