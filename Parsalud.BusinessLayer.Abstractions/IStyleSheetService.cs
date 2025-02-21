using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IStyleSheetService : IManagerServiceBase<ParsaludStyleSheet, StyleSheetSearchCriteria>
{
    Task<BusinessResponse<string>> GetBundleCssAsync(CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludStyleSheet>> CreateAsync(ManageStyleSheetRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludStyleSheet>> GetByFileNameAsync(string fileName, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludStyleSheet>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse> UpdateAsync(Guid id, ManageStyleSheetRequest request, CancellationToken cancellationToken = default);
}