namespace Parsalud.BusinessLayer.Abstractions;

public class ManageFaqRequest
{
    public required string Question { get; init; }
    public required string Answer { get; init; }
    public required bool Hidden { get; init; }
    public required Guid? ServiceId { get; init; }
}
