using IMDB.Application.Common.Exceptions;
using IMDB.Application.Common.Models;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Genres;

public sealed record GetGenresQuery(int? Page, int? PageSize) : IRequest<PagedResult<GenreDto>>;

public sealed class GetGenresQueryHandler(IGenreRepository genres)
    : IRequestHandler<GetGenresQuery, PagedResult<GenreDto>>
{
    public Task<PagedResult<GenreDto>> Handle(GetGenresQuery request, CancellationToken cancellationToken)
    {
        return genres.GetResultAsync(request.Page, request.PageSize, g => g.ToDto(), cancellationToken);
    }
}

public sealed record GetGenreQuery(int Id) : IRequest<GenreDto>;

public sealed class GetGenreQueryHandler(IGenreRepository genres) : IRequestHandler<GetGenreQuery, GenreDto>
{
    public async Task<GenreDto> Handle(GetGenreQuery request, CancellationToken cancellationToken)
    {
        var genre = await genres.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Genre not found");

        return genre.ToDto();
    }
}
