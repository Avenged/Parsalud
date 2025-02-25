using Castle.Core.Logging;
using Microsoft.Extensions.Logging;
using Parsalud.BusinessLayer.Abstractions;
using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer;

[GenerateHub(useAuthentication: true)]
public class FileService : IFileService
{
    private readonly string _appStoragePath;
    private readonly ILogger<FileService> _logger;

    public FileService(ILogger<FileService> logger)
    {
        _logger = logger;

        string basePath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        _appStoragePath = Path.Combine(basePath, "ParsaludWebApp", "uploads");

        try
        {
            if (!Directory.Exists(_appStoragePath))
            {
                Directory.CreateDirectory(_appStoragePath);
            }
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "No se pudo crear el directorio de almacenamiento de archivos");
            // Do not throw
        }
    }

    public Task<BusinessResponse<ParsaludFile>> CreateAsync(ManageFileRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<BusinessResponse> DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            await Task.CompletedTask;

            // Obtener solo el nombre del archivo sin rutas maliciosas
            fileName = Path.GetFileName(fileName);
            var filePath = Path.Combine(_appStoragePath, fileName);

            // Normalizar la ruta y verificar que esté dentro del directorio permitido
            var fullPath = Path.GetFullPath(filePath);
            if (!fullPath.StartsWith(_appStoragePath, StringComparison.OrdinalIgnoreCase))
            {
                return BusinessResponse.Error("Acceso no permitido.");
            }

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return BusinessResponse.Success();
            }
            else
            {
                return BusinessResponse.Error("No se encontró el archivo.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo eliminar el archivo.");
            return BusinessResponse.Error("Ocurrió un error inesperado.");
        }
    }

    public Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public async Task<BusinessResponse<Paginated<ParsaludFile[]>>> GetByCriteriaAsync(FileSearchCriteria criteria, CancellationToken cancellationToken = default)
    {
        try
        {
            var files = Directory.GetFiles(_appStoragePath);

            var entries = files.Select(filePath => new ParsaludFile
            {
                Id = Guid.NewGuid(),
                FileName = Path.GetFileName(filePath),
            }).ToArray();

            if (!string.IsNullOrWhiteSpace(criteria.FileName))
                entries = entries.Where(x => x.FileName.Contains(criteria.FileName)).ToArray();

            await Task.CompletedTask;

            return BusinessResponse.Success(new Paginated<ParsaludFile[]>()
            {
                Data = entries,
                Page = 0,
                PageSize = entries.Length,
                TotalItems = entries.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "No se pudo listar los archivos del almacenamiento: {AppStoragePath}", _appStoragePath);
            return BusinessResponse.Error<Paginated<ParsaludFile[]>>("Ocurrió un error inesperado");
        }
    }

    public Task<BusinessResponse<ParsaludFile>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }

    public Task<BusinessResponse<ParsaludFile>> UpdateAsync(Guid id, ManageFileRequest request, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException();
    }
}
