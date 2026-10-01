using IMDB.Domain.Common;

namespace IMDB.Domain.Entities;

public sealed class Series : Media, IAggregateRoot
{
    private readonly List<Genre> _genres = new();
    private readonly List<Season> _seasons = new();

    private Series()
    {
    }

    public IReadOnlyCollection<Genre> Genres => _genres;
    public IReadOnlyCollection<Season> Seasons => _seasons;

    public static Series Create(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        var series = new Series();
        series.Apply(imdbId, title, releaseDate, description, posterUrl, rate, genres);
        return series;
    }

    public void Update(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        Apply(imdbId, title, releaseDate, description, posterUrl, rate, genres);
    }

    public Season AddSeason(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int seasonNumber,
        int episodesCount)
    {
        if (_seasons.Any(s => s.SeasonNumber == seasonNumber))
            throw new DomainException($"Season {seasonNumber} already exists in this series");

        var season = Season.Create(imdbId, title, releaseDate, description, posterUrl, rate, seasonNumber, episodesCount);
        _seasons.Add(season);
        return season;
    }

    public Season UpdateSeason(
        int seasonId,
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int seasonNumber,
        int episodesCount)
    {
        var season = GetSeason(seasonId);

        if (_seasons.Any(s => s.Id != seasonId && s.SeasonNumber == seasonNumber))
            throw new DomainException($"Season {seasonNumber} already exists in this series");

        season.Update(imdbId, title, releaseDate, description, posterUrl, rate, seasonNumber, episodesCount);
        return season;
    }

    public Season RemoveSeason(int seasonId)
    {
        var season = GetSeason(seasonId);
        _seasons.Remove(season);
        return season;
    }

    public Episode AddEpisode(
        int seasonId,
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int episodeNumber,
        int? durationMinutes)
    {
        return GetSeason(seasonId).AddEpisode(imdbId, title, releaseDate, description, posterUrl, rate, episodeNumber, durationMinutes);
    }

    public Episode UpdateEpisode(
        int seasonId,
        int episodeId,
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int episodeNumber,
        int? durationMinutes)
    {
        return GetSeason(seasonId).UpdateEpisode(episodeId, imdbId, title, releaseDate, description, posterUrl, rate, episodeNumber, durationMinutes);
    }

    public Episode RemoveEpisode(int seasonId, int episodeId)
    {
        return GetSeason(seasonId).RemoveEpisode(episodeId);
    }

    private Season GetSeason(int seasonId)
    {
        return _seasons.FirstOrDefault(s => s.Id == seasonId)
               ?? throw new DomainException("Season does not belong to this series");
    }

    private void Apply(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        var genreList = genres.ToList();

        if (genreList.Count == 0)
            throw new DomainException("A series needs at least one genre");

        SetDetails(imdbId, title, releaseDate, description, posterUrl, rate);

        _genres.Clear();
        _genres.AddRange(genreList);
    }
}
