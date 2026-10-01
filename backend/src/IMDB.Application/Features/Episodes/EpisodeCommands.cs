using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Episodes;

public sealed record EpisodeData(
    int SeasonId,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int EpisodeNumber,
    int? DurationMinutes,
    string PosterUrl,
    double Rate);

public sealed record CreateEpisodeCommand(EpisodeData Data) : IRequest<EpisodeDto>;

public sealed class CreateEpisodeCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateEpisodeCommand, EpisodeDto>
{
    public async Task<EpisodeDto> Handle(CreateEpisodeCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var series = await seriesRepository.GetBySeasonIdAsync(data.SeasonId, cancellationToken)
                     ?? throw new NotFoundException("Invalid Season ID");

        var episode = series.AddEpisode(
            data.SeasonId,
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            data.EpisodeNumber,
            data.DurationMinutes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return episode.ToDto();
    }
}

public sealed record UpdateEpisodeCommand(int Id, EpisodeData Data) : IRequest<EpisodeDto>;

public sealed class UpdateEpisodeCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateEpisodeCommand, EpisodeDto>
{
    public async Task<EpisodeDto> Handle(UpdateEpisodeCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var series = await seriesRepository.GetByEpisodeIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Episode not found");

        var currentSeasonId = series.Seasons.First(s => s.Episodes.Any(e => e.Id == request.Id)).Id;

        if (currentSeasonId != data.SeasonId)
            throw new DomainException("An episode cannot be moved to another season");

        var episode = series.UpdateEpisode(
            currentSeasonId,
            request.Id,
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            data.EpisodeNumber,
            data.DurationMinutes);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return episode.ToDto();
    }
}

public sealed record DeleteEpisodeCommand(int Id) : IRequest;

public sealed class DeleteEpisodeCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteEpisodeCommand>
{
    public async Task Handle(DeleteEpisodeCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetByEpisodeIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Episode not found");

        var seasonId = series.Seasons.First(s => s.Episodes.Any(e => e.Id == request.Id)).Id;
        var episode = series.RemoveEpisode(seasonId, request.Id);

        seriesRepository.RemoveEpisode(episode);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
