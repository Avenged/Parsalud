using Parsalud.Components;
using Radzen;
using Parsalud.Extensions;
using Parsalud.BusinessLayer;
using VENative.Blazor.ServiceGenerator.Extensions;
using Microsoft.AspNetCore.Identity;
using Parsalud.DataAccess.Models;
using Parsalud.Components.Account;
using Microsoft.AspNetCore.Mvc;
using Parsalud.BusinessLayer.Abstractions;
using Microsoft.AspNetCore.Antiforgery;
using Humanizer;
using ZiggyCreatures.Caching.Fusion;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRadzenComponents();
builder.Services.AddFusionCache().WithDefaultEntryOptions(options =>
{
    options.Duration = TimeSpan.MaxValue;
});
builder.Services.AddMemoryCache();
builder.Services.AddBusinessLayer(builder.Configuration);
builder.Services.AddSecurity();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery();

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

app.MapPost("/contactme", async (
    HttpContext context,
    [FromForm] IFormCollection form, 
    IAntiforgery antiforgery) =>
{
    if (!string.IsNullOrWhiteSpace(form["__RequestVerificationToken"]))
    {
        context.Request.Headers.TryAdd("X-CSRF-TOKEN", form["__RequestVerificationToken"]);
        if (!antiforgery.IsRequestValidAsync(context).Result)
        {
            return Results.BadRequest("Token CSRF inválido");
        }
    }
    else
    {
        return Results.BadRequest("Token CSRF inválido");
    }

    string? givenName = form["given-name"].FirstOrDefault()?.Truncate(100);
    string? familyName = form["family-name"].FirstOrDefault()?.Truncate(100);
    string? tel = form["tel"].FirstOrDefault()?.Truncate(20);
    string? mail = form["email"].FirstOrDefault()?.Truncate(50);
    string? comments = form["comments"].FirstOrDefault()?.Truncate(500);

    try
    {
        //MimeMessage email = new();
        //email.From.Add(new MailboxAddress("Parsalud", "notificaciones@tuempresa.com"));
        //email.To.Add(new MailboxAddress("Asesor", "asesor@tuempresa.com"));
        //email.Subject = "Nuevo Contacto de Cliente";

        //email.Body = new TextPart("plain")
        //{
        //    Text = 
        //    $"""
        //    Nuevo cliente ha enviado un formulario:

        //    - Nombre: {givenName} {familyName}
        //    - Teléfono: {tel}
        //    - Email: {mail}
        //    - Comentarios: {comments}

        //    Contactar lo antes posible.
        //    """
        //};

        //using var smtp = new SmtpClient();
        //await smtp.ConnectAsync("smtp.tudominio.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        //await smtp.AuthenticateAsync("notificaciones@tuempresa.com", "tucontraseña");
        //await smtp.SendAsync(email);
        //await smtp.DisconnectAsync(true);

        return Results.LocalRedirect("~/Contacto/Success");
    }
    catch (Exception ex)
    {
        return Results.Problem("Error al enviar el correo: " + ex.Message);
    }
});

app.MapGeneratedHubs(useAuthorization: true, typeof(Parsalud.BusinessLayer.IAssemblyMarker).Assembly);
app.MapStaticAssets();
app.MapGet($"/css/{AppConstants.MAIN_CSS_FILENAME}", async ([FromServices] IStyleSheetService ssService, CancellationToken cancellationToken = default) =>
{
    var css = await ssService.GetBundleCssAsync(cancellationToken);
    if (css.IsSuccessWithData)
    {
        return Results.Text(css.Data, "text/css");
    }

    return Results.Text("", "text/css");
});
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Parsalud.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();