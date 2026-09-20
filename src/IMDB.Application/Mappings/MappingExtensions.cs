using IMDB.Application.DTOs;
using IMDB.Domain.Entities;

namespace IMDB.Application.Mappings;

public static class MappingExtensions
{
    public static GenreDto ToDto(this Genre genre)
    {
        return new GenreDto(genre.Id, genre.Title);
    }

    public static MovieDto ToDto(this Movie movie)
    {
        return new MovieDto(
            movie.Id,
            movie.ImdbId.Value,
            movie.Title,
            movie.ReleaseDate,
            movie.Description,
            movie.Duration,
            movie.Genres.Select(g => g.ToDto()).ToList(),
            movie.PosterUrl,
            movie.Rate.Value);
    }

    public static SeriesDto ToDto(this Series series)
    {
        return new SeriesDto(
            series.Id,
            series.ImdbId.Value,
            series.Title,
            series.ReleaseDate,
            series.Description,
            series.Genres.Select(g => g.ToDto()).ToList(),
            series.PosterUrl,
            series.Rate.Value);
    }

    public static SeasonDto ToDto(this Season season)
    {
        return new SeasonDto(
            season.Id,
            season.ImdbId.Value,
            season.Title,
            season.ReleaseDate,
            season.Description,
            season.SeasonNumber,
            season.EpisodesCount,
            season.SeriesId,
            season.PosterUrl,
            season.Rate.Value,
            season.Episodes.Select(e => e.ToDto()).ToList());
    }

    public static EpisodeDto ToDto(this Episode episode)
    {
        return new EpisodeDto(
            episode.Id,
            episode.ImdbId.Value,
            episode.Title,
            episode.ReleaseDate,
            episode.Description,
            episode.EpisodeNumber,
            episode.DurationMinutes,
            episode.SeasonId,
            episode.PosterUrl,
            episode.Rate.Value);
    }

    public static PersonDto ToDisplayDto(this Person person)
    {
        return new PersonDto(person.Id, person.ImdbId.Value, person.FullName, person.BirthDate, null, person.PhotoUrl);
    }

    public static PersonDto ToDetailDto(this Person person)
    {
        return new PersonDto(person.Id, person.ImdbId.Value, person.FullName, person.BirthDate, person.Bio, person.PhotoUrl);
    }

    public static CastDto ToDto(this Cast cast, Person? person = null)
    {
        var owner = cast.Person ?? person;

        return new CastDto(cast.Id, cast.MediaId, cast.PersonId, cast.Role, owner?.ToDisplayDto());
    }

    public static CommentDto ToDto(this Comment comment, string? userName)
    {
        return new CommentDto(
            comment.Id,
            comment.Text,
            comment.MediaId,
            comment.UserId,
            userName,
            comment.CreatedOn,
            comment.LastUpdatedOn);
    }

    public static RateDto ToDto(this Rate rate, string? userName)
    {
        return new RateDto(rate.Id, rate.MediaId, rate.UserId, userName, rate.Score);
    }
}
