using Parsalud.BusinessLayer.Abstractions;

namespace Parsalud.Client.Pages.Files;

public class FileModel : IModelBase<FileModel, ManageFileRequest, ParsaludFile>
{
    public Guid? Id { get; set; }
    public string? FileName { get; set; }

    public FileModel FromDto(ParsaludFile dto)
    {
        return new FileModel
        {
            Id = dto.Id,
            FileName = dto.FileName
        };
    }

    public ManageFileRequest ToRequest()
    {
        return new ManageFileRequest
        {
            Id = Id ?? Guid.Empty,
            FileName = FileName ?? string.Empty
        };
    }
}
