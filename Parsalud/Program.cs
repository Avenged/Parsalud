using Parsalud.Components;
using Radzen;
using Parsalud.Extensions;
using Parsalud.BusinessLayer;
using VENative.Blazor.ServiceGenerator.Extensions;
using Microsoft.AspNetCore.Identity;
using Parsalud.DataAccess.Models;
using Parsalud.Components.Account;
using Parsalud;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var useDashboard = configuration.GetValue<bool>("UseDashboard");

builder.Services.Configure<EmailSettings>(configuration.GetRequiredSection("EmailSettings"));

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog(options =>
    {
        options.LogName = "Application";
        options.SourceName = "Parsalud Web App";
    });
}

if (configuration.GetValue<bool>("UseAntiforgery"))
{
    builder.Services.AddAntiforgery();
}

builder.Services.AddRadzenComponents();
builder.Services.AddMemoryCache();
builder.Services.AddBusinessLayer(builder.Configuration);
builder.Services.AddSecurity();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();
builder.Services.AddScoped<SignInManager<ParsaludUser>, CustomSignInManager<ParsaludUser>>();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(c =>
    {
        c.DetailedErrors = builder.Environment.IsDevelopment();
    })
    .AddInteractiveWebAssemblyComponents()
    .AddAuthenticationStateSerialization();

builder.Services.AddSingleton<IEmailSender<ParsaludUser>, IdentityNoOpEmailSender>();

var app = builder.Build();

app.UsePathBase(configuration.GetValue<string>("Base"));

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

if (configuration.GetValue<bool>("UseHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

if (configuration.GetValue<bool>("UseAntiforgery"))
{
    app.UseAntiforgery();
}

app.MapStaticAssets();
app.MapContactMe();
app.MapCssBundle();
app.MapUpload();
app.MapUploads();

if (!useDashboard)
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/Dashboard") ||
            context.Request.Path.StartsWithSegments("/Account"))
        {
            context.Response.StatusCode = 404;
            return;
        }

        await next();
    });
}
else
{
    app.Use(async (context, next) =>
    {
        if (context.Request.Path.StartsWithSegments("/"))
        {
            context.Response.Redirect("/Dashboard");
            return;
        }

        await next();
    });
}

app.MapGeneratedHubs(useAuthorization: true, typeof(Parsalud.BusinessLayer.IAssemblyMarker).Assembly);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Parsalud.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();