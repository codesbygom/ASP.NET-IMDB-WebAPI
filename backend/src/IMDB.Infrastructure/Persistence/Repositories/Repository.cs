using IMDB.Domain.Common;
using IMDB.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Infrastructure.Persistence.Repositories;

public abstract class Repository<T>(ApplicationDbContext context) : IRepository<T>
    where T : Entity, IAggregateRoot
{
    protected ApplicationDbContext Context { get; } = context;

    protected virtual IQueryable<T> Query() => Context.Set<T>();

    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await Query().FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public virtual async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Query().AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);
    }

    public virtual async Task<(IReadOnlyList<T> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var total = await Context.Set<T>().CountAsync(cancellationToken);

        var items = await Query()
            .AsNoTracking()
            .OrderBy(e => e.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }

    public virtual void Add(T entity) => Context.Set<T>().Add(entity);

    public virtual void Remove(T entity) => Context.Set<T>().Remove(entity);
}
