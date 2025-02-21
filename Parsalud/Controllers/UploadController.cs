using Microsoft.AspNetCore.Mvc;
using Parsalud.BusinessLayer.Abstractions;
using System.IO;
using System.Threading.Tasks;

namespace Parsalud.Controllers;

[Route("upload")]
[ApiController]
public class UploadController : ControllerBase
{
    private readonly string _appStoragePath;

    public UploadController()
    {
        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        _appStoragePath = Path.Combine(basePath, "ParsaludWebApp", "uploads");

        if (!Directory.Exists(_appStoragePath))
        {
            Directory.CreateDirectory(_appStoragePath);
        }
    }

    [HttpPost("single")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No se ha subido ningún archivo.");
        }

        var filePath = Path.Combine(_appStoragePath, file.FileName);
        using (var fileStream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(fileStream);
        }

        return Ok(new { Message = "Archivo subido correctamente.", FilePath = filePath });
    }
}

