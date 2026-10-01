using IMDB.Domain.Entities;

namespace IMDB.Domain.Repositories;

public interface ISeriesRepository : IRepository<Series>
{
    Task<Series?> GetWithSeasonsAsync(int seriesId, CancellationToken cancellationToken = default);

    Task<Series?> GetBySeasonIdAsync(int seasonId, CancellationToken cancellationToken = default);

    Task<Series?> GetByEpisodeIdAsync(int episodeId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Season>> GetAllSeasonsAsync(CancellationToken cancellationToken = default);

    Task<Season?> GetSeasonAsync(int seasonId, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Episode>> GetAllEpisodesAsync(CancellationToken cancellationToken = default);

    Task<Episode?> GetEpisodeAsync(int episodeId, CancellationToken cancellationToken = default);

    void RemoveSeason(Season season);

    void RemoveEpisode(Episode episode);
}
