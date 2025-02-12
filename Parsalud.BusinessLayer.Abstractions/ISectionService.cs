using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface ISectionService
{
    Task<BusinessResponse> CreateAsync(ManageSectionRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> UpdateAsync(Guid id, ManageSectionRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludSection[]>> GetByCriteriaAsync(SectionSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludSection>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludSection>> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
}