using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Infrastructure.Persistence.Repositories;

public sealed class GenreRepository(ApplicationDbContext context) : Repository<Genre>(context), IGenreRepository
{
    public async Task<IReadOnlyList<Genre>> GetByIdsAsync(IEnumerable<int> ids, CancellationToken cancellationToken = default)
    {
        var idList = ids.ToList();

        return await Context.Set<Genre>().Where(g => idList.Contains(g.Id)).ToListAsync(cancellationToken);
    }
}

public sealed class MovieRepository(ApplicationDbContext context) : Repository<Movie>(context), IMovieRepository
{
    protected override IQueryable<Movie> Query() => Context.Set<Movie>().Include(m => m.Genres);
}

public sealed class PersonRepository(ApplicationDbContext context) : Repository<Person>(context), IPersonRepository
{
}

public sealed class CastRepository(ApplicationDbContext context) : Repository<Cast>(context), ICastRepository
{
    protected override IQueryable<Cast> Query() => Context.Set<Cast>().Include(c => c.Person);

    public async Task<IReadOnlyList<Cast>> GetByMediaIdAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        return await Query()
            .AsNoTracking()
            .Where(c => c.MediaId == mediaId)
            .OrderBy(c => c.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IReadOnlyList<Cast> Items, int TotalCount)> GetPagedByMediaIdAsync(
        int mediaId,
        int page,
        int pageSize,
        CancellationToken cancellationToken = default)
    {
        var query = Query().AsNoTracking().Where(c => c.MediaId == mediaId);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, total);
    }
}

public sealed class CommentRepository(ApplicationDbContext context) : Repository<Comment>(context), ICommentRepository
{
    public async Task<IReadOnlyList<Comment>> GetByMediaIdAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Comment>()
            .AsNoTracking()
            .Where(c => c.MediaId == mediaId)
            .OrderByDescending(c => c.CreatedOn)
            .ToListAsync(cancellationToken);
    }
}

public sealed class RateRepository(ApplicationDbContext context) : Repository<Rate>(context), IRateRepository
{
    public async Task<IReadOnlyList<Rate>> GetByMediaIdAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Rate>()
            .AsNoTracking()
            .Where(r => r.MediaId == mediaId)
            .OrderByDescending(r => r.CreatedOn)
            .ToListAsync(cancellationToken);
    }
}

public sealed class MediaRepository(ApplicationDbContext context) : IMediaRepository
{
    public Task<bool> ExistsAsync(int mediaId, CancellationToken cancellationToken = default)
    {
        return context.Set<Media>().AnyAsync(m => m.Id == mediaId, cancellationToken);
    }
}
