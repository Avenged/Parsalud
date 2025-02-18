using System.Collections;

namespace Parsalud.BusinessLayer.Abstractions;

public class Paginated<T>
    where T : IEnumerable
{
    public required T Data { get; init; }
    public required int? Page { get; init; }
    public required int? PageSize { get; init; }
    public required int TotalItems { get; init; }

    public static implicit operator T(Paginated<T> paginated) => paginated.Data;
}
