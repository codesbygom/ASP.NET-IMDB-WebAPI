using IMDB.Domain.Enums;

namespace IMDB.Application.DTOs;

public sealed record GenreDto(int Id, string Title);

public sealed record MovieDto(
    int Id,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int Duration,
    IReadOnlyList<GenreDto> Genres,
    string PosterUrl,
    double Rate);

public sealed record SeriesDto(
    int Id,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    IReadOnlyList<GenreDto> Genres,
    string PosterUrl,
    double Rate);

public sealed record SeasonDto(
    int Id,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int SeasonNumber,
    int EpisodesCount,
    int SeriesId,
    string PosterUrl,
    double Rate,
    IReadOnlyList<EpisodeDto> Episodes);

public sealed record EpisodeDto(
    int Id,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int EpisodeNumber,
    int? DurationMinutes,
    int SeasonId,
    string PosterUrl,
    double Rate);

public sealed record PersonDto(
    int Id,
    string ImdbId,
    string FullName,
    DateTime BirthDate,
    string? Bio,
    string PhotoUrl);

public sealed record CastDto(int Id, int MediaId, int PersonId, CastRole Role, PersonDto? Person);

public sealed record CommentDto(
    int Id,
    string Text,
    int ContentId,
    string UserId,
    string? UserName,
    DateTime CreatedOn,
    DateTime LastUpdatedOn);

public sealed record RateDto(int Id, int MediaId, string UserId, string? UserName, ScoreEnum Score);

public sealed record NewUserDto(string UserName, string Email, string Token);
