using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IFaqService
{
    Task<BusinessResponse> CreateAsync(ManageFaqRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludFaq[]>> GetByCriteriaAsync(FaqSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludFaq>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse> UpdateAsync(Guid id, ManageFaqRequest request, CancellationToken cancellationToken = default);
}