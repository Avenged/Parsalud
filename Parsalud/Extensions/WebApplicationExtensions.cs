using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.Options;
using Parsalud.BusinessLayer.Abstractions;
using System.Net;
using System.Net.Mail;
using System.Text;

namespace Parsalud.Extensions;

public static class WebApplicationExtensions
{
    public static void MapContactMe(this WebApplication app)
    {
        app.MapPost("/contactme", async (
            HttpContext context,
            [FromForm] IFormCollection form,
            IOptions<EmailSettings> settings,
            ILogger<Program> Logger) =>
        {
            EmailSettings emailSettings = settings.Value;
            string fromEmail = emailSettings.Username;
            string password = emailSettings.Password;
            string emailFrom = emailSettings.EmailFrom;
            string hostName = emailSettings.HostName;
            string emailTo = emailSettings.EmailTo;
            string subject = emailSettings.Subject;
            string host = emailSettings.Host;
            int port = emailSettings.Port;
            string template = emailSettings.Template;
            bool enableSsl = emailSettings.EnableSsl;
            bool useDefaultCredentials = emailSettings.UseDefaultCredentials;

            string? givenName = form["given-name"].FirstOrDefault()?.Truncate(100);
            string? familyName = form["family-name"].FirstOrDefault()?.Truncate(100);
            string? tel = form["tel"].FirstOrDefault()?.Truncate(20);
            string? mail = form["email"].FirstOrDefault()?.Truncate(50);
            string? comments = form["comments"].FirstOrDefault()?.Truncate(500);

            string?[] strs = [givenName, familyName, tel, mail];

            if (strs.Any(x => string.IsNullOrWhiteSpace(x)))
            {
                return Results.BadRequest("Formulario incorrecto. Corrija los datos enviados");
            }

            template = template.Replace("{GivenName}", givenName);
            template = template.Replace("{FamilyName}", familyName);
            template = template.Replace("{Tel}", tel);
            template = template.Replace("{Mail}", mail);
            template = template.Replace("{Comments}", comments);

            try
            {
                MailMessage message = new()
                {
                    From = new MailAddress(emailFrom, hostName, Encoding.UTF8),
                    Subject = subject,
                };
                message.To.Add(new MailAddress(emailTo));
                message.Body = template;
                message.IsBodyHtml = true;

                SmtpClient smtpClient = new()
                {
                    Host = host,
                    Port = port,
                    UseDefaultCredentials = useDefaultCredentials,
                    Credentials = new NetworkCredential(fromEmail, password),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = enableSsl,
                };
                await smtpClient.SendMailAsync(message);

                return Results.LocalRedirect("~/dr/Contacto/Success");
            }
            catch (Exception ex)
            {
                Logger.LogCritical(ex, "No se pudo enviar el mail de contacto");
                return Results.Problem(
                    title: "InternalServerError",
                    detail: "Presentamos problemas para procesar tu solicitud.", 
                    statusCode: StatusCodes.Status500InternalServerError);
            }
        });
    }

    public static void MapCssBundle(this WebApplication app)
    {
        app.MapGet($"/css/{AppConstants.MAIN_CSS_FILENAME}", async ([FromServices] IStyleSheetService ssService, CancellationToken cancellationToken = default) =>
        {
            var css = await ssService.GetBundleCssAsync(cancellationToken);
            if (css.IsSuccessWithData)
            {
                return Results.Text(css.Data, "text/css");
            }

            return Results.Text("", "text/css");
        });
    }

    public static void MapUpload(this WebApplication app)
    {
        app.MapPost("upload/single", async (
            IFormFile file, 
            HttpContext context) =>
        {
            if (file is null || file.Length == 0)
            {
                return Results.BadRequest("No se ha subido ningún archivo.");
            }

            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var _appStoragePath = Path.Combine(basePath, "ParsaludWebApp", "uploads");

            if (!Directory.Exists(_appStoragePath))
            {
                Directory.CreateDirectory(_appStoragePath);
            }

            // Obtener solo el nombre del archivo sin rutas maliciosas
            string fileName = Path.GetFileName(file.FileName);
            string filePath = Path.Combine(_appStoragePath, fileName);

            // Normalizar la ruta y verificar que esté dentro del directorio permitido
            string fullPath = Path.GetFullPath(filePath);
            if (!fullPath.StartsWith(_appStoragePath, StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Acceso no permitido.");
            }

            // Validar extensiones permitidas
            HashSet<string> allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
            string fileExtension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return Results.BadRequest("Tipo de archivo no permitido.");
            }

            // Guardar el archivo de manera segura
            using (FileStream fileStream = new(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return Results.Ok(new { Message = "Archivo subido correctamente.", FilePath = fullPath });
        });
    }

    public static void MapUploads(this WebApplication app)
    {
        app.MapGet("uploads/{fileName}", async (
            string fileName,
            HttpContext context) =>
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return Results.BadRequest("El nombre del archivo es requerido.");
            }

            string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            var appStoragePath = Path.Combine(basePath, "ParsaludWebApp", "uploads");
            var contentTypeProvider = new FileExtensionContentTypeProvider();

            // Asegurar que solo se use el nombre del archivo sin rutas relativas
            fileName = Path.GetFileName(fileName);
            string filePath = Path.Combine(appStoragePath, fileName);

            // Normalizar la ruta y verificar que esté dentro del directorio permitido
            string fullPath = Path.GetFullPath(filePath);
            if (!fullPath.StartsWith(appStoragePath, StringComparison.OrdinalIgnoreCase))
            {
                return Results.BadRequest("Acceso no permitido.");
            }

            if (!File.Exists(fullPath))
            {
                return Results.NotFound("Archivo no encontrado.");
            }

            if (!contentTypeProvider.TryGetContentType(fullPath, out var contentType))
            {
                contentType = "application/octet-stream";
            }

            byte[] fileBytes = await File.ReadAllBytesAsync(fullPath);
            return Results.File(fileBytes, contentType);
        });
    }
}
