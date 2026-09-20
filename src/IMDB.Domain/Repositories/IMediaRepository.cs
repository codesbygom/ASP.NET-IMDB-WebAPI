namespace IMDB.Domain.Repositories;

public interface IMediaRepository
{
    Task<bool> ExistsAsync(int mediaId, CancellationToken cancellationToken = default);
}
