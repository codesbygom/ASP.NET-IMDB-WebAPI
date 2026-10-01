using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Features.Genres;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Movies;

public sealed record MovieData(
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int Duration,
    string PosterUrl,
    double Rate,
    IReadOnlyList<int> GenreIds);

public sealed record CreateMovieCommand(MovieData Data) : IRequest<MovieDto>;

public sealed class CreateMovieCommandHandler(IMovieRepository movies, IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateMovieCommand, MovieDto>
{
    public async Task<MovieDto> Handle(CreateMovieCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        var movieGenres = await GenreLookup.ResolveAsync(genres, data.GenreIds, cancellationToken);

        var movie = Movie.Create(
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.Duration,
            data.PosterUrl,
            data.Rate,
            movieGenres);

        movies.Add(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return movie.ToDto();
    }
}

public sealed record UpdateMovieCommand(int Id, MovieData Data) : IRequest<MovieDto>;

public sealed class UpdateMovieCommandHandler(IMovieRepository movies, IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateMovieCommand, MovieDto>
{
    public async Task<MovieDto> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var movie = await movies.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Movie not found");

        var movieGenres = await GenreLookup.ResolveAsync(genres, data.GenreIds, cancellationToken);

        movie.Update(
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.Duration,
            data.PosterUrl,
            data.Rate,
            movieGenres);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return movie.ToDto();
    }
}

public sealed record DeleteMovieCommand(int Id) : IRequest;

public sealed class DeleteMovieCommandHandler(IMovieRepository movies, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteMovieCommand>
{
    public async Task Handle(DeleteMovieCommand request, CancellationToken cancellationToken)
    {
        var movie = await movies.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Movie not found");

        movies.Remove(movie);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
