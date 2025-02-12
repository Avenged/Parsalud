using Parsalud.Components;
using Radzen;
using Parsalud.Extensions;
using Parsalud.BusinessLayer;
using VENative.Blazor.ServiceGenerator.Extensions;
using Microsoft.AspNetCore.Identity;
using Parsalud.DataAccess.Models;
using Parsalud.Components.Account;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRadzenComponents();

builder.Services.AddBusinessLayer(builder.Configuration);
builder.Services.AddSecurity();
builder.Services.AddHttpContextAccessor();

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

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapGeneratedHubs(useAuthorization: true, typeof(Parsalud.BusinessLayer.IAssemblyMarker).Assembly);
app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Parsalud.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();