using IMDB.Domain.Common;

namespace IMDB.Domain.Entities;

public sealed class Episode : Media
{
    private Episode()
    {
    }

    public int EpisodeNumber { get; private set; }
    public int? DurationMinutes { get; private set; }
    public int SeasonId { get; private set; }

    internal static Episode Create(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int episodeNumber,
        int? durationMinutes)
    {
        var episode = new Episode();
        episode.Apply(imdbId, title, releaseDate, description, posterUrl, rate, episodeNumber, durationMinutes);
        return episode;
    }

    internal void Update(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int episodeNumber,
        int? durationMinutes)
    {
        Apply(imdbId, title, releaseDate, description, posterUrl, rate, episodeNumber, durationMinutes);
    }

    private void Apply(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate,
        int episodeNumber,
        int? durationMinutes)
    {
        SetDetails(imdbId, title, releaseDate, description, posterUrl, rate);
        EpisodeNumber = Guard.InRange(episodeNumber, 1, 1000, "Episode number");
        DurationMinutes = durationMinutes.HasValue
            ? Guard.InRange(durationMinutes.Value, 1, 300, "Duration")
            : null;
    }
}
