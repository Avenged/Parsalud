using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Parsalud.BusinessLayer.Abstractions;

public interface IUserService
{
    void SetUser(ClaimsPrincipal? user);
    Guid Id { get; }
    string? Name { get; }
    string? Email { get; }
}