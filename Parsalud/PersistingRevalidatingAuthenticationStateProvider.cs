using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Parsalud.DataAccess.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace Parsalud;

// This is a server-side AuthenticationStateProvider that revalidates the security stamp for the connected user
// every 30 minutes an interactive circuit is connected. It also uses PersistentComponentState to flow the
// authentication state to the client which is then fixed for the lifetime of the WebAssembly application.
internal sealed class PersistingRevalidatingAuthenticationStateProvider : RevalidatingServerAuthenticationStateProvider
{
    private readonly IServiceScopeFactory scopeFactory;
    private readonly PersistentComponentState state;
    private readonly IdentityOptions options;

    private readonly PersistingComponentStateSubscription subscription;

    private Task<AuthenticationState>? authenticationStateTask;

    public PersistingRevalidatingAuthenticationStateProvider(
        ILoggerFactory loggerFactory,
        IServiceScopeFactory serviceScopeFactory,
        PersistentComponentState persistentComponentState,
        IOptions<IdentityOptions> optionsAccessor)
        : base(loggerFactory)
    {
        scopeFactory = serviceScopeFactory;
        state = persistentComponentState;
        options = optionsAccessor.Value;

        AuthenticationStateChanged += OnAuthenticationStateChanged;
        subscription = state.RegisterOnPersisting(OnPersistingAsync, RenderMode.InteractiveWebAssembly);
    }

    protected override TimeSpan RevalidationInterval => TimeSpan.FromMinutes(30);

    protected override async Task<bool> ValidateAuthenticationStateAsync(
        AuthenticationState authenticationState, CancellationToken cancellationToken)
    {
        // Get the user manager from a new scope to ensure it fetches fresh data
        await using var scope = scopeFactory.CreateAsyncScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ParsaludUser>>();
        return await ValidateSecurityStampAsync(userManager, authenticationState.User);
    }

    private async Task<bool> ValidateSecurityStampAsync(UserManager<ParsaludUser> userManager, ClaimsPrincipal principal)
    {
        var user = await userManager.GetUserAsync(principal);
        if (user is null)
        {
            return false;
        }
        else if (!userManager.SupportsUserSecurityStamp)
        {
            return true;
        }
        else
        {
            var principalStamp = principal.FindFirstValue(options.ClaimsIdentity.SecurityStampClaimType);
            var userStamp = await userManager.GetSecurityStampAsync(user);
            return principalStamp == userStamp;
        }
    }

    private void OnAuthenticationStateChanged(Task<AuthenticationState> task)
    {
        authenticationStateTask = task;
    }

    private async Task OnPersistingAsync()
    {
        if (authenticationStateTask is null)
        {
            throw new UnreachableException($"Authentication state not set in {nameof(OnPersistingAsync)}().");
        }

        var authenticationState = await authenticationStateTask;
        var principal = authenticationState.User;

        if (principal.Identity?.IsAuthenticated == true)
        {
            var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var name = principal.FindFirst(ClaimTypes.Name)?.Value;
            var givenName = principal.FindFirst(ClaimTypes.GivenName)?.Value;
            var mobilePhone = principal.FindFirst(ClaimTypes.MobilePhone)?.Value;
            var homePhone = principal.FindFirst(ClaimTypes.HomePhone)?.Value;
            var surname = principal.FindFirst(ClaimTypes.Surname)?.Value;
            var email = principal.FindFirst(ClaimTypes.Email)?.Value;
            var roles = principal.FindAll(ClaimTypes.Role)
                .Select(x => x.Value).ToArray();

            if (nameIdentifier != null && email != null && name != null)
            {
                state.PersistAsJson(nameof(UserInfo), new UserInfo
                {
                    NameIdentifier = nameIdentifier,
                    Name = name,
                    GivenName = givenName,
                    MobilePhone = mobilePhone,
                    Surname = surname,
                    HomePhone = homePhone,
                    Email = email,
                    Roles = roles,
                });
            }
        }
    }

    protected override void Dispose(bool disposing)
    {
        subscription.Dispose();
        AuthenticationStateChanged -= OnAuthenticationStateChanged;
        base.Dispose(disposing);
    }
}