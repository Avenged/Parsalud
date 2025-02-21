namespace Parsalud.BusinessLayer.Abstractions;

public interface IManagerServiceBase<TDto, TRequest, TCriteria>
    where TDto : class
    where TRequest : class
    where TCriteria : class, new()
{
    Task<BusinessResponse<TDto>> CreateAsync(TRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse<TDto>> UpdateAsync(Guid id, TRequest request, CancellationToken cancellationToken = default);
    Task<BusinessResponse<Paginated<TDto[]>>> GetByCriteriaAsync(TCriteria criteria, CancellationToken cancellationToken = default);
    Task<BusinessResponse<TDto>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<BusinessResponse> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}