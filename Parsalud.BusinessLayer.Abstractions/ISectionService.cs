﻿using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface ISectionService : IManagerServiceBase<ParsaludSection, ManageSectionRequest, SectionSearchCriteria>
{
    Task UpdateLiveServerAsync(LiveServerInstance ins, CancellationToken cancellationToken = default);
    Task<BusinessResponse<ParsaludSection>> GetByCodeAsync(
        string code,
        string? param1 = default,
        string? param2 = default,
        string? param3 = default,
        string? param4 = default,
        string? param5 = default,
        string? param6 = default,
        CancellationToken cancellationToken = default);
}