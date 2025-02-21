using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Controllers;

[Route("uploads")]
[ApiController]
public class UploadsController : ControllerBase
{
    private readonly string _appStoragePath;
    private readonly FileExtensionContentTypeProvider _contentTypeProvider;

    public UploadsController()
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        _appStoragePath = Path.Combine(basePath, "ParsaludWebApp", "uploads");
        _contentTypeProvider = new FileExtensionContentTypeProvider();
    }

    [HttpGet("{fileName}")]
    public async Task<IActionResult> GetFile(string fileName)
    {
        if (string.IsNullOrEmpty(fileName))
        {
            return BadRequest("El nombre del archivo es requerido.");
        }

        var filePath = Path.Combine(_appStoragePath, fileName);

        if (!System.IO.File.Exists(filePath))
        {
            return NotFound("Archivo no encontrado.");
        }

        if (!_contentTypeProvider.TryGetContentType(filePath, out var contentType))
        {
            contentType = "application/octet-stream";
        }

        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
        return File(fileBytes, contentType);
    }
}