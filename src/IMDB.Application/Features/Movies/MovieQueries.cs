using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Movies;

public sealed record GetMoviesQuery : IRequest<IReadOnlyList<MovieDto>>;

public sealed class GetMoviesQueryHandler(IMovieRepository movies)
    : IRequestHandler<GetMoviesQuery, IReadOnlyList<MovieDto>>
{
    public async Task<IReadOnlyList<MovieDto>> Handle(GetMoviesQuery request, CancellationToken cancellationToken)
    {
        var items = await movies.GetAllAsync(cancellationToken);

        return items.Select(m => m.ToDto()).ToList();
    }
}

public sealed record GetMovieQuery(int Id) : IRequest<MovieDto>;

public sealed class GetMovieQueryHandler(IMovieRepository movies) : IRequestHandler<GetMovieQuery, MovieDto>
{
    public async Task<MovieDto> Handle(GetMovieQuery request, CancellationToken cancellationToken)
    {
        var movie = await movies.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Movie not found");

        return movie.ToDto();
    }
}
