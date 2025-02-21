namespace Parsalud.BusinessLayer.Abstractions;

public class ManageUserRequest
{
    public required bool LockoutEnabled { get; init; }
}
