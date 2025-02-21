using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Parsalud.DataAccess.Models;

namespace Parsalud;

public class CustomSignInManager<TUser> : SignInManager<TUser> where TUser : ParsaludUser
{
    public CustomSignInManager(
        UserManager<TUser> userManager,
        IHttpContextAccessor contextAccessor,
        IUserClaimsPrincipalFactory<TUser> claimsFactory,
        IOptions<IdentityOptions> optionsAccessor,
        ILogger<SignInManager<TUser>> logger,
        IAuthenticationSchemeProvider schemes,
        IUserConfirmation<TUser> confirmation)
        : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
    {
    }

    public override async Task<SignInResult> CheckPasswordSignInAsync(TUser user, string password, bool lockoutOnFailure)
    {
        if (user.IsDisabled)
        {
            return SignInResult.NotAllowed;
        }

        return await base.CheckPasswordSignInAsync(user, password, lockoutOnFailure);
    }
}

