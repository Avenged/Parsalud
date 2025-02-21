using Microsoft.AspNetCore.Identity;

namespace Parsalud.DataAccess.Models;

public class ParsaludUser : IdentityUser<Guid>
{
    public bool IsDisabled { get; set; } = false;
}