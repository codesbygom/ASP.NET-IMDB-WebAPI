using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Enums;
using IMDB.Domain.ValueObjects;

namespace IMDB.Tests.Domain;

public class DomainTests
{
    private const string Url = "https://example.com/poster.jpg";
    private static readonly DateTime Date = new(2020, 1, 1);

    [Fact]
    public void Genre_requires_a_title_of_at_least_two_characters()
    {
        Assert.Throws<DomainException>(() => Genre.Create("A"));
        Assert.Throws<DomainException>(() => Genre.Create("   "));
        Assert.Equal("Drama", Genre.Create("  Drama ").Title);
    }

    [Theory]
    [InlineData(-0.1)]
    [InlineData(10.1)]
    [InlineData(double.NaN)]
    public void Rating_rejects_values_outside_the_scale(double value)
    {
        Assert.Throws<DomainException>(() => Rating.From(value));
    }

    [Fact]
    public void Rating_rounds_to_one_decimal()
    {
        Assert.Equal(8.7, Rating.From(8.66).Value);
    }

    [Fact]
    public void Movie_requires_at_least_one_genre()
    {
        Assert.Throws<DomainException>(() =>
            Movie.Create("tt1", "Title", Date, "A long description", 120, Url, 8, Array.Empty<Genre>()));
    }

    [Fact]
    public void Movie_rejects_invalid_poster_url()
    {
        var genre = Genre.Create("Drama");

        Assert.Throws<DomainException>(() =>
            Movie.Create("tt1", "Title", Date, "A long description", 120, "not-a-url", 8, new[] { genre }));
    }

    [Fact]
    public void Series_rejects_duplicate_season_numbers()
    {
        var series = CreateSeries();
        series.AddSeason("tt2", "Season 1", Date, "A long description", Url, 8, 1, 10);

        Assert.Throws<DomainException>(() =>
            series.AddSeason("tt3", "Season 1 again", Date, "A long description", Url, 8, 1, 10));
    }

    [Fact]
    public void Season_rejects_duplicate_episode_numbers()
    {
        var series = CreateSeries();
        var season = series.AddSeason("tt2", "Season 1", Date, "A long description", Url, 8, 1, 10);

        series.AddEpisode(season.Id, "tt3", "Pilot", Date, "A long description", Url, 8, 1, 45);

        Assert.Throws<DomainException>(() =>
            series.AddEpisode(season.Id, "tt4", "Pilot copy", Date, "A long description", Url, 8, 1, 45));
    }

    [Fact]
    public void Comment_can_only_be_edited_by_its_author()
    {
        var comment = Comment.Create(1, "user-1", "Great");

        Assert.Throws<ForbiddenDomainException>(() => comment.Edit("user-2", "Hacked"));

        comment.Edit("user-1", "Updated");
        Assert.Equal("Updated", comment.Text);
    }

    [Fact]
    public void Rate_can_only_be_changed_by_its_author()
    {
        var rate = Rate.Create(1, "user-1", ScoreEnum.Three);

        Assert.Throws<ForbiddenDomainException>(() => rate.ChangeScore("user-2", ScoreEnum.One));

        rate.ChangeScore("user-1", ScoreEnum.Five);
        Assert.Equal(ScoreEnum.Five, rate.Score);
    }

    [Fact]
    public void Person_cannot_be_born_in_the_future()
    {
        Assert.Throws<DomainException>(() =>
            Person.Create("nm1", "Jane Doe", DateTime.UtcNow.AddDays(2), "A long biography", Url));
    }

    private static Series CreateSeries()
    {
        return Series.Create("tt1", "Series", Date, "A long description", Url, 8, new[] { Genre.Create("Drama") });
    }
}
