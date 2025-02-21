using VENative.Blazor.ServiceGenerator.Attributes;

namespace Parsalud.BusinessLayer.Abstractions;

[GenerateClient]
public interface IPostService : IManagerServiceBase<ParsaludPost, ManagePostRequest, PostSearchCriteria>
{
    Task<BusinessResponse<ParsaludPost[]>> GetLatestAsync(CancellationToken cancellationToken = default);
}