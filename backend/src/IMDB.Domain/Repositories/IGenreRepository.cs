using IMDB.Domain.Entities;

namespace IMDB.Domain.Repositories;

public interface IGenreRepository : IRepository<Genre>
{
    Task<IReadOnlyList<Genre>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default);
}
