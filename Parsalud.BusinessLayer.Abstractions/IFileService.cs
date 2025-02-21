using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IFileService : IManagerServiceBase<ParsaludFile, ManageFileRequest, FileSearchCriteria>
{
    Task<BusinessResponse> DeleteByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
}