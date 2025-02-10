namespace Parsalud.BusinessLayer.Abstractions;

public interface IUserService
{
    Guid Id { get; }
    string? Name { get; }
    string? Email { get; }
}