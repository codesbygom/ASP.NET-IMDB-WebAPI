using IMDB.Domain.Entities;

namespace IMDB.Domain.Repositories;

public interface ICastRepository : IRepository<Cast>
{
    Task<IReadOnlyList<Cast>> GetByMediaIdAsync(int mediaId, CancellationToken cancellationToken = default);

    Task<(IReadOnlyList<Cast> Items, int TotalCount)> GetPagedByMediaIdAsync(int mediaId, int page, int pageSize, CancellationToken cancellationToken = default);
}
