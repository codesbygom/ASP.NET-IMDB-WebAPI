using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Seasons;

public sealed record GetSeasonsQuery : IRequest<IReadOnlyList<SeasonDto>>;

public sealed class GetSeasonsQueryHandler(ISeriesRepository seriesRepository)
    : IRequestHandler<GetSeasonsQuery, IReadOnlyList<SeasonDto>>
{
    public async Task<IReadOnlyList<SeasonDto>> Handle(GetSeasonsQuery request, CancellationToken cancellationToken)
    {
        var seasons = await seriesRepository.GetAllSeasonsAsync(cancellationToken);

        return seasons.Select(s => s.ToDto()).ToList();
    }
}

public sealed record GetSeasonQuery(int Id) : IRequest<SeasonDto>;

public sealed class GetSeasonQueryHandler(ISeriesRepository seriesRepository) : IRequestHandler<GetSeasonQuery, SeasonDto>
{
    public async Task<SeasonDto> Handle(GetSeasonQuery request, CancellationToken cancellationToken)
    {
        var season = await seriesRepository.GetSeasonAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Season not found");

        return season.ToDto();
    }
}
