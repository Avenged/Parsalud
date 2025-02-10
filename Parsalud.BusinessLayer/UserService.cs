using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.BusinessLayer;

public class UserService(IHttpContextAccessor httpContextAccessor) : IUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public Guid Id 
    {
        get
        {
            var nameIdentifier = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (Guid.TryParse(nameIdentifier, out var id))
            {
                return id;
            }

            throw new InvalidOperationException("User is not authenticated");
        }
    }

    public string? Name
    {
        get
        {
           return  _httpContextAccessor.HttpContext?.User?.Identity?.Name;
        } 
    }

    public string? Email
    {
        get
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}