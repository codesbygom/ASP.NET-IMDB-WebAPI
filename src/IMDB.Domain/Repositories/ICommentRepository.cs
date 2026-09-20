using IMDB.Domain.Entities;

namespace IMDB.Domain.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<IReadOnlyList<Comment>> GetByMediaIdAsync(int mediaId, CancellationToken cancellationToken = default);
}
