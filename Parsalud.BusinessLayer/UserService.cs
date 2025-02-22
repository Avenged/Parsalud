using System.Security.Claims;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.BusinessLayer;

public class UserService : IUserService
{
    private ClaimsPrincipal? _user;

    public void SetUser(ClaimsPrincipal? user)
    {
        _user = user;
    }

    public Guid Id 
    {
        get
        {
            var nameIdentifier = _user?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

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
           return  _user?.Identity?.Name;
        } 
    }

    public string? Email
    {
        get
        {
            return _user?.FindFirst(ClaimTypes.Email)?.Value;
        }
    }
}