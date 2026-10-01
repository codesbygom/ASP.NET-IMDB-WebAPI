using IMDB.Domain.Common;

namespace IMDB.Domain.Entities;

public sealed class Movie : Media, IAggregateRoot
{
    private readonly List<Genre> _genres = new();

    private Movie()
    {
    }

    public int Duration { get; private set; }
    public IReadOnlyCollection<Genre> Genres => _genres;

    public static Movie Create(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        int duration,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        var movie = new Movie();
        movie.Apply(imdbId, title, releaseDate, description, duration, posterUrl, rate, genres);
        return movie;
    }

    public void Update(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        int duration,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        Apply(imdbId, title, releaseDate, description, duration, posterUrl, rate, genres);
    }

    private void Apply(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        int duration,
        string posterUrl,
        double rate,
        IEnumerable<Genre> genres)
    {
        var genreList = genres.ToList();

        if (genreList.Count == 0)
            throw new DomainException("A movie needs at least one genre");

        SetDetails(imdbId, title, releaseDate, description, posterUrl, rate);
        Duration = Guard.InRange(duration, 1, 600, "Duration");

        _genres.Clear();
        _genres.AddRange(genreList);
    }
}
