using Microsoft.AspNetCore.Identity;
using Parsalud.DataAccess.Models;

namespace Parsalud.Components.Account;

internal sealed class IdentityUserAccessor(UserManager<ParsaludUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<ParsaludUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Incapaz de cargar al usuario con ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}
