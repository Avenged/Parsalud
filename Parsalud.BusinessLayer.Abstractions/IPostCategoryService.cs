using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IPostCategoryService
{
    Task<BusinessResponse> CreateAsync(ManagePostCategoryRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludPostCategory[]>> GetByCriteriaAsync(PostCategorySearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludPostCategory>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse> UpdateAsync(Guid id, ManagePostCategoryRequest request, CancellationToken cancellationToken = default);
}