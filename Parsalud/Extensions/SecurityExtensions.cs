using Microsoft.AspNetCore.Components.Authorization;
using Parsalud.DataAccess.Models;
using Parsalud.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.Cookies;
using Parsalud.Components.Account;

namespace Parsalud.Extensions;

public static class SecurityExtensions
{
    public static void AddSecurity(this IServiceCollection services)
    {
        services.AddScoped<IdentityUserAccessor>();
        services.AddScoped<IdentityRedirectManager>();
        services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
        services.AddIdentityCore<ParsaludUser>(options =>
        {
            options.SignIn.RequireConfirmedAccount = false;

            options.Password.RequireNonAlphanumeric = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.Password.RequireDigit = false;

            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            options.Lockout.MaxFailedAccessAttempts = 5;
            options.Lockout.AllowedForNewUsers = true;
        })
            .AddErrorDescriber<CustomIdentityErrorDescriber>()
            .AddEntityFrameworkStores<ParsaludDbContext>()
            .AddSignInManager()
            .AddDefaultTokenProviders();

        services.AddAuthentication(options =>
        {
            options.DefaultScheme = IdentityConstants.ApplicationScheme;
        })
            .AddIdentityCookies();

        //services.AddOptions<CookieAuthenticationOptions>(IdentityConstants.ApplicationScheme)
        //    .Configure<ITicketStore>((options, store) =>
        //    {
        //        options.SessionStore = store;
        //    });

        services.AddScoped<SignInManager<ParsaludUser>, SignInManager<ParsaludUser>>();
        services.AddTransient<IUserClaimsPrincipalFactory<ParsaludUser>, UserClaimsPrincipalFactory<ParsaludUser>>();
        services.AddCascadingAuthenticationState();
    }
}