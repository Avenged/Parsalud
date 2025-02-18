﻿using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IPostService
{
    Task<BusinessResponse<ParsaludPost[]>> GetLatestAsync(CancellationToken cancellationToken = default);
    Task<BusinessResponse<Paginated<ParsaludPost[]>>> GetByCriteriaAsync(PostSearchCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludPost>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse> CreateAsync(ManagePostRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> UpdateAsync(Guid id, ManagePostRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}