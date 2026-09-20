using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Infrastructure.Persistence.Repositories;

public sealed class SeriesRepository(ApplicationDbContext context) : Repository<Series>(context), ISeriesRepository
{
    protected override IQueryable<Series> Query() => Context.Set<Series>().Include(s => s.Genres);

    private IQueryable<Series> QueryWithSeasons()
    {
        return Query().Include(s => s.Seasons).ThenInclude(season => season.Episodes);
    }

    public async Task<Series?> GetWithSeasonsAsync(int seriesId, CancellationToken cancellationToken = default)
    {
        return await QueryWithSeasons().FirstOrDefaultAsync(s => s.Id == seriesId, cancellationToken);
    }

    public async Task<Series?> GetBySeasonIdAsync(int seasonId, CancellationToken cancellationToken = default)
    {
        return await QueryWithSeasons()
            .FirstOrDefaultAsync(s => s.Seasons.Any(season => season.Id == seasonId), cancellationToken);
    }

    public async Task<Series?> GetByEpisodeIdAsync(int episodeId, CancellationToken cancellationToken = default)
    {
        return await QueryWithSeasons()
            .FirstOrDefaultAsync(
                s => s.Seasons.Any(season => season.Episodes.Any(episode => episode.Id == episodeId)),
                cancellationToken);
    }

    public async Task<IReadOnlyList<Season>> GetAllSeasonsAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Season>()
            .Include(s => s.Episodes)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .ToListAsync(cancellationToken);
    }

    public async Task<Season?> GetSeasonAsync(int seasonId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Season>()
            .Include(s => s.Episodes)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == seasonId, cancellationToken);
    }

    public async Task<IReadOnlyList<Episode>> GetAllEpisodesAsync(CancellationToken cancellationToken = default)
    {
        return await Context.Set<Episode>().AsNoTracking().OrderBy(e => e.Id).ToListAsync(cancellationToken);
    }

    public async Task<Episode?> GetEpisodeAsync(int episodeId, CancellationToken cancellationToken = default)
    {
        return await Context.Set<Episode>().AsNoTracking().FirstOrDefaultAsync(e => e.Id == episodeId, cancellationToken);
    }

    public void RemoveSeason(Season season)
    {
        Context.Set<Episode>().RemoveRange(season.Episodes);
        Context.Set<Season>().Remove(season);
    }

    public void RemoveEpisode(Episode episode)
    {
        Context.Set<Episode>().Remove(episode);
    }

    public override void Remove(Series series)
    {
        foreach (var season in series.Seasons)
            RemoveSeason(season);

        base.Remove(series);
    }
}
