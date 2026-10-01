using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Series;

public sealed record GetSeriesListQuery : IRequest<IReadOnlyList<SeriesDto>>;

public sealed class GetSeriesListQueryHandler(ISeriesRepository seriesRepository)
    : IRequestHandler<GetSeriesListQuery, IReadOnlyList<SeriesDto>>
{
    public async Task<IReadOnlyList<SeriesDto>> Handle(GetSeriesListQuery request, CancellationToken cancellationToken)
    {
        var items = await seriesRepository.GetAllAsync(cancellationToken);

        return items.Select(s => s.ToDto()).ToList();
    }
}

public sealed record GetSeriesQuery(int Id) : IRequest<SeriesDto>;

public sealed class GetSeriesQueryHandler(ISeriesRepository seriesRepository) : IRequestHandler<GetSeriesQuery, SeriesDto>
{
    public async Task<SeriesDto> Handle(GetSeriesQuery request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Series not found");

        return series.ToDto();
    }
}
