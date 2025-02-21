namespace Parsalud.BusinessLayer.Abstractions;

public interface IManagerServiceBase<TDto, TCriteria>
    where TDto : class
    where TCriteria : class, new()
{
    Task<BusinessResponse<Paginated<TDto[]>>> GetByCriteriaAsync(TCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}