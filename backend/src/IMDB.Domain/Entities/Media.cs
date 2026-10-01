using IMDB.Domain.Common;
using IMDB.Domain.ValueObjects;

namespace IMDB.Domain.Entities;

public abstract class Media : Entity
{
    protected Media()
    {
        ImdbId = null!;
        Title = null!;
        Description = null!;
        PosterUrl = null!;
        Rate = null!;
    }

    public ImdbId ImdbId { get; private set; }
    public string Title { get; private set; }
    public DateTime ReleaseDate { get; private set; }
    public string Description { get; private set; }
    public string PosterUrl { get; private set; }
    public Rating Rate { get; private set; }

    protected void SetDetails(
        string imdbId,
        string title,
        DateTime releaseDate,
        string description,
        string posterUrl,
        double rate)
    {
        ImdbId = ImdbId.From(imdbId);
        Title = Guard.NotEmpty(title, "Title", 200);
        ReleaseDate = releaseDate;
        Description = Guard.NotEmpty(description, "Description", 2000);
        PosterUrl = Guard.Url(posterUrl, "Poster URL");
        Rate = Rating.From(rate);
    }
}
