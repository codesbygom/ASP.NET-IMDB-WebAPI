using IMDB.Domain.Common;

namespace IMDB.Domain.Repositories;

public interface IRepository<T> where T : Entity, IAggregateRoot
{
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, CancellationToken cancellationToken = default);

    void Add(T entity);

    void Remove(T entity);
}
