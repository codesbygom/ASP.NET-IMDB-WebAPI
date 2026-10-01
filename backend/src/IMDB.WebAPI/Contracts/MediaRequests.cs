using System.ComponentModel.DataAnnotations;
using IMDB.Application.Features.Episodes;
using IMDB.Application.Features.Movies;
using IMDB.Application.Features.Seasons;
using IMDB.Application.Features.Series;

namespace IMDB.WebAPI.Contracts;

public class MovieRequest
{
    [Required(ErrorMessage = "IMDB ID is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "IMDB ID must be between 1 and 20 characters")]
    public string ImdbId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Duration is required")]
    [Range(1, 600, ErrorMessage = "Duration must be between 1 and 600 minutes")]
    public int Duration { get; set; }

    [MinLength(1, ErrorMessage = "At least one Genre ID is required")]
    public List<int> GenreIds { get; set; } = new();

    [Required(ErrorMessage = "Poster URL is required")]
    [Url(ErrorMessage = "Invalid URL format for poster")]
    [StringLength(500, ErrorMessage = "Poster URL must not exceed 500 characters")]
    public string PosterUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rate is required")]
    [Range(0, 10, ErrorMessage = "Rate must be between 0 and 10")]
    public double Rate { get; set; }

    public MovieData ToData() => new(ImdbId, Title, ReleaseDate, Description, Duration, PosterUrl, Rate, GenreIds);
}

public class SeriesRequest
{
    [Required(ErrorMessage = "IMDB ID is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "IMDB ID must be between 1 and 20 characters")]
    public string ImdbId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [MinLength(1, ErrorMessage = "At least one Genre ID is required")]
    public List<int> GenreIds { get; set; } = new();

    [Required(ErrorMessage = "Poster URL is required")]
    [Url(ErrorMessage = "Invalid URL format for poster")]
    [StringLength(500, ErrorMessage = "Poster URL must not exceed 500 characters")]
    public string PosterUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rate is required")]
    [Range(0, 10, ErrorMessage = "Rate must be between 0 and 10")]
    public double Rate { get; set; }

    public SeriesData ToData() => new(ImdbId, Title, ReleaseDate, Description, PosterUrl, Rate, GenreIds);
}

public class SeasonRequest
{
    [Required(ErrorMessage = "IMDB ID is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "IMDB ID must be between 1 and 20 characters")]
    public string ImdbId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Season number is required")]
    [Range(1, 100, ErrorMessage = "Season number must be between 1 and 100")]
    public int SeasonNumber { get; set; }

    [Required(ErrorMessage = "Episodes count is required")]
    [Range(1, 50, ErrorMessage = "Episodes count must be between 1 and 50")]
    public int EpisodesCount { get; set; }

    [Required(ErrorMessage = "Series ID is required")]
    public int SeriesId { get; set; }

    [Required(ErrorMessage = "Poster URL is required")]
    [Url(ErrorMessage = "Invalid URL format for poster")]
    [StringLength(500, ErrorMessage = "Poster URL must not exceed 500 characters")]
    public string PosterUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rate is required")]
    [Range(0, 10, ErrorMessage = "Rate must be between 0 and 10")]
    public double Rate { get; set; }

    public SeasonData ToData() => new(SeriesId, ImdbId, Title, ReleaseDate, Description, SeasonNumber, EpisodesCount, PosterUrl, Rate);
}

public class EpisodeRequest
{
    [Required(ErrorMessage = "IMDB ID is required")]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "IMDB ID must be between 1 and 20 characters")]
    public string ImdbId { get; set; } = string.Empty;

    [Required(ErrorMessage = "Title is required")]
    [StringLength(200, MinimumLength = 1, ErrorMessage = "Title must be between 1 and 200 characters")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Release date is required")]
    public DateTime ReleaseDate { get; set; }

    [Required(ErrorMessage = "Description is required")]
    [StringLength(2000, MinimumLength = 10, ErrorMessage = "Description must be between 10 and 2000 characters")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Episode number is required")]
    [Range(1, 1000, ErrorMessage = "Episode number must be between 1 and 1000")]
    public int EpisodeNumber { get; set; }

    [Range(1, 300, ErrorMessage = "Duration must be between 1 and 300 minutes")]
    public int? DurationMinutes { get; set; }

    [Required(ErrorMessage = "Season ID is required")]
    public int SeasonId { get; set; }

    [Required(ErrorMessage = "Poster URL is required")]
    [Url(ErrorMessage = "Invalid URL format for poster")]
    [StringLength(500, ErrorMessage = "Poster URL must not exceed 500 characters")]
    public string PosterUrl { get; set; } = string.Empty;

    [Required(ErrorMessage = "Rate is required")]
    [Range(0, 10, ErrorMessage = "Rate must be between 0 and 10")]
    public double Rate { get; set; }

    public EpisodeData ToData() => new(SeasonId, ImdbId, Title, ReleaseDate, Description, EpisodeNumber, DurationMinutes, PosterUrl, Rate);
}
