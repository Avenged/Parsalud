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
using Parsalud;
using NUglify.Helpers;
using MimeKit;
using NuGet.Configuration;
using Microsoft.Extensions.Options;
using MailKit.Net.Smtp;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

if (OperatingSystem.IsWindows())
{
    builder.Logging.AddEventLog(options =>
    {
        options.LogName = "Application";
        options.SourceName = "Parsalud Web App";
    });
}

builder.Services.AddControllers();
builder.Services.AddSingleton<IParsaludWebHostEnvironment>(x =>
{
    var env = x.GetRequiredService<IWebHostEnvironment>();
    return new ParsaludWebHostEnvironment(env.WebRootPath);
});
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

if (configuration.GetValue<bool>("UseHttpsRedirection"))
{
    app.UseHttpsRedirection();
}

app.UseAntiforgery();
app.MapStaticAssets();
app.MapControllers();
app.MapPost("/contactme", async (
    HttpContext context,
    [FromForm] IFormCollection form, 
    IOptions<EmailSettings> settings,
    ILogger<Program> Logger,
    IAntiforgery antiforgery) =>
{
    var emailFrom = settings.Value.EmailFrom;
    var emailTo = settings.Value.EmailTo;
    var subject = settings.Value.Subject;
    var username = settings.Value.Username;
    var password = settings.Value.Password;
    var host = settings.Value.Host;
    var port = settings.Value.Port;
    var template = settings.Value.Template;

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

    string?[] strs = [givenName, familyName, tel, mail];

    if (strs.Any(x => string.IsNullOrWhiteSpace(x)))
    {
        return Results.BadRequest("Formulario incorrecto");
    }

    template = template.Replace("{GivenName}", givenName);
    template = template.Replace("{FamilyName}", familyName);
    template = template.Replace("{Tel}", tel);
    template = template.Replace("{Mail}", mail);
    template = template.Replace("{Comments}", comments);

    try
    {
        MimeMessage email = new();
        email.From.Add(new MailboxAddress("Parsalud Web App", emailFrom));
        email.To.Add(new MailboxAddress("Asesor", emailTo));
        email.Subject = subject;

        email.Body = new TextPart("plain")
        {
            Text = template
        };

        using SmtpClient smtp = new();
        await smtp.ConnectAsync(host, port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(username, password);
        await smtp.SendAsync(email);
        await smtp.DisconnectAsync(true);

        return Results.LocalRedirect("~/Contacto/Success");
    }
    catch (Exception ex)
    {
        Logger.LogCritical(ex, "No se pudo enviar el mail de contacto");
        return Results.Problem("Error al enviar el correo: " + ex.Message);
    }
});

app.MapGet($"/css/{AppConstants.MAIN_CSS_FILENAME}", async ([FromServices] IStyleSheetService ssService, CancellationToken cancellationToken = default) =>
{
    
    var css = await ssService.GetBundleCssAsync(cancellationToken);
    if (css.IsSuccessWithData)
    {
        return Results.Text(css.Data, "text/css");
    }

    return Results.Text("", "text/css");
});

app.MapGeneratedHubs(useAuthorization: true, typeof(Parsalud.BusinessLayer.IAssemblyMarker).Assembly);

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Parsalud.Client._Imports).Assembly);

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

await app.RunAsync();