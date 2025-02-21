using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IStyleSheetService : IManagerServiceBase<ParsaludStyleSheet, ManageStyleSheetRequest, StyleSheetSearchCriteria>
{
    Task<BusinessResponse<string>> GetBundleCssAsync(CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludStyleSheet>> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
}