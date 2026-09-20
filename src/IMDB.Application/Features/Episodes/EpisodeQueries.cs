using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Episodes;

public sealed record GetEpisodesQuery : IRequest<IReadOnlyList<EpisodeDto>>;

public sealed class GetEpisodesQueryHandler(ISeriesRepository seriesRepository)
    : IRequestHandler<GetEpisodesQuery, IReadOnlyList<EpisodeDto>>
{
    public async Task<IReadOnlyList<EpisodeDto>> Handle(GetEpisodesQuery request, CancellationToken cancellationToken)
    {
        var episodes = await seriesRepository.GetAllEpisodesAsync(cancellationToken);

        return episodes.Select(e => e.ToDto()).ToList();
    }
}

public sealed record GetEpisodeQuery(int Id) : IRequest<EpisodeDto>;

public sealed class GetEpisodeQueryHandler(ISeriesRepository seriesRepository) : IRequestHandler<GetEpisodeQuery, EpisodeDto>
{
    public async Task<EpisodeDto> Handle(GetEpisodeQuery request, CancellationToken cancellationToken)
    {
        var episode = await seriesRepository.GetEpisodeAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException("Episode not found");

        return episode.ToDto();
    }
}
