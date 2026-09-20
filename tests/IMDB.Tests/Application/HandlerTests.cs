using IMDB.Application.Common.Exceptions;
using IMDB.Application.Features.Comments;
using IMDB.Application.Features.Episodes;
using IMDB.Application.Features.Genres;
using IMDB.Application.Features.Movies;
using IMDB.Application.Features.Seasons;
using IMDB.Application.Features.Series;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Infrastructure.Identity;
using IMDB.Infrastructure.Persistence.Repositories;
using IMDB.Tests.Support;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Tests.Application;

public class HandlerTests
{
    private const string Url = "https://example.com/poster.jpg";
    private static readonly DateTime Date = new(2020, 1, 1);

    [Fact]
    public async Task Genre_crud_round_trip()
    {
        using var db = new TestDatabase();
        var genres = new GenreRepository(db.Context);

        var created = await new CreateGenreCommandHandler(genres, db.Context)
            .Handle(new CreateGenreCommand("Drama"), default);

        await new UpdateGenreCommandHandler(genres, db.Context)
            .Handle(new UpdateGenreCommand(created.Id, "Comedy"), default);

        db.Context.ChangeTracker.Clear();

        var loaded = await new GetGenreQueryHandler(genres).Handle(new GetGenreQuery(created.Id), default);
        Assert.Equal("Comedy", loaded.Title);

        await new DeleteGenreCommandHandler(genres, db.Context).Handle(new DeleteGenreCommand(created.Id), default);

        await Assert.ThrowsAsync<NotFoundException>(() =>
            new GetGenreQueryHandler(genres).Handle(new GetGenreQuery(created.Id), default));
    }

    [Fact]
    public async Task Movie_creation_rejects_unknown_genres()
    {
        using var db = new TestDatabase();
        var handler = new CreateMovieCommandHandler(new MovieRepository(db.Context), new GenreRepository(db.Context), db.Context);

        var data = new MovieData("tt1", "Title", Date, "A long description", 120, Url, 8, new[] { 999 });

        await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new CreateMovieCommand(data), default));
    }

    [Fact]
    public async Task Series_aggregate_can_be_built_and_deleted_with_its_children()
    {
        using var db = new TestDatabase();
        var context = db.Context;
        var genres = new GenreRepository(context);
        var series = new SeriesRepository(context);

        var genre = await new CreateGenreCommandHandler(genres, context).Handle(new CreateGenreCommand("Drama"), default);

        var createdSeries = await new CreateSeriesCommandHandler(series, genres, context).Handle(
            new CreateSeriesCommand(new SeriesData("tt1", "Series", Date, "A long description", Url, 8, new[] { genre.Id })),
            default);

        context.ChangeTracker.Clear();

        var season = await new CreateSeasonCommandHandler(series, context).Handle(
            new CreateSeasonCommand(new SeasonData(createdSeries.Id, "tt2", "Season 1", Date, "A long description", 1, 10, Url, 8)),
            default);

        context.ChangeTracker.Clear();

        var episode = await new CreateEpisodeCommandHandler(series, context).Handle(
            new CreateEpisodeCommand(new EpisodeData(season.Id, "tt3", "Pilot", Date, "A long description", 1, 45, Url, 8)),
            default);

        context.ChangeTracker.Clear();

        var loadedSeason = await new GetSeasonQueryHandler(series).Handle(new GetSeasonQuery(season.Id), default);
        Assert.Single(loadedSeason.Episodes);
        Assert.Equal(episode.Id, loadedSeason.Episodes[0].Id);

        await new DeleteSeriesCommandHandler(series, context).Handle(new DeleteSeriesCommand(createdSeries.Id), default);

        Assert.Equal(0, await context.Set<Media>().CountAsync());
    }

    [Fact]
    public async Task Episode_and_season_can_be_deleted_individually()
    {
        using var db = new TestDatabase();
        var context = db.Context;
        var genres = new GenreRepository(context);
        var series = new SeriesRepository(context);

        var genre = await new CreateGenreCommandHandler(genres, context).Handle(new CreateGenreCommand("Drama"), default);
        var createdSeries = await new CreateSeriesCommandHandler(series, genres, context).Handle(
            new CreateSeriesCommand(new SeriesData("tt1", "Series", Date, "A long description", Url, 8, new[] { genre.Id })),
            default);
        context.ChangeTracker.Clear();

        var season = await new CreateSeasonCommandHandler(series, context).Handle(
            new CreateSeasonCommand(new SeasonData(createdSeries.Id, "tt2", "Season 1", Date, "A long description", 1, 10, Url, 8)),
            default);
        context.ChangeTracker.Clear();

        var episode = await new CreateEpisodeCommandHandler(series, context).Handle(
            new CreateEpisodeCommand(new EpisodeData(season.Id, "tt3", "Pilot", Date, "A long description", 1, 45, Url, 8)),
            default);
        context.ChangeTracker.Clear();

        await new DeleteEpisodeCommandHandler(series, context).Handle(new DeleteEpisodeCommand(episode.Id), default);
        context.ChangeTracker.Clear();

        Assert.Equal(0, await context.Set<Episode>().CountAsync());

        await new DeleteSeasonCommandHandler(series, context).Handle(new DeleteSeasonCommand(season.Id), default);
        context.ChangeTracker.Clear();

        Assert.Equal(0, await context.Set<Season>().CountAsync());
        Assert.Equal(1, await context.Set<Series>().CountAsync());
    }

    [Fact]
    public async Task Only_the_author_can_edit_a_comment()
    {
        using var db = new TestDatabase();
        var context = db.Context;
        var identity = new FakeIdentityService();

        context.Users.Add(new AppUser { Id = "user-1", UserName = "alice", Email = "alice@example.com" });
        context.Users.Add(new AppUser { Id = "user-2", UserName = "bob", Email = "bob@example.com" });
        await context.SaveChangesAsync();

        var genres = new GenreRepository(context);
        var genre = await new CreateGenreCommandHandler(genres, context).Handle(new CreateGenreCommand("Drama"), default);
        var movie = await new CreateMovieCommandHandler(new MovieRepository(context), genres, context).Handle(
            new CreateMovieCommand(new MovieData("tt1", "Title", Date, "A long description", 120, Url, 8, new[] { genre.Id })),
            default);
        context.ChangeTracker.Clear();

        var comments = new CommentRepository(context);
        var media = new MediaRepository(context);

        var created = await new AddCommentCommandHandler(comments, media, identity, new FakeCurrentUser("user-1"), context)
            .Handle(new AddCommentCommand(movie.Id, "Loved it"), default);

        Assert.Equal("name-of-user-1", created.UserName);
        context.ChangeTracker.Clear();

        var otherUser = new UpdateCommentCommandHandler(comments, identity, new FakeCurrentUser("user-2"), context);

        await Assert.ThrowsAsync<ForbiddenDomainException>(() =>
            otherUser.Handle(new UpdateCommentCommand(created.Id, "Hijacked"), default));

        var author = new UpdateCommentCommandHandler(comments, identity, new FakeCurrentUser("user-1"), context);
        var updated = await author.Handle(new UpdateCommentCommand(created.Id, "Edited"), default);

        Assert.Equal("Edited", updated.Text);
    }

    [Fact]
    public async Task Comment_requires_an_authenticated_user()
    {
        using var db = new TestDatabase();
        var handler = new AddCommentCommandHandler(
            new CommentRepository(db.Context),
            new MediaRepository(db.Context),
            new FakeIdentityService(),
            new FakeCurrentUser(null),
            db.Context);

        await Assert.ThrowsAsync<UnauthorizedException>(() => handler.Handle(new AddCommentCommand(1, "Hi"), default));
    }
}
