namespace Parsalud.BusinessLayer.Abstractions;

public record ParsaludService(Guid Id, string Name, string Code) : IDtoBase;