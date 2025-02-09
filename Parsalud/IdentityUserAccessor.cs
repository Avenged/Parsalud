using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Parsalud.DataAccess.Models;

namespace Parsalud;

internal sealed class IdentityUserAccessor(UserManager<ParsaludUser> userManager, IdentityRedirectManager redirectManager)
{
    public async Task<ParsaludUser> GetRequiredUserAsync(HttpContext context)
    {
        var user = await userManager.GetUserAsync(context.User);

        if (user is null)
        {
            redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
        }

        return user;
    }
}