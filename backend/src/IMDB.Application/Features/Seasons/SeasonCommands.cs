using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Seasons;

public sealed record SeasonData(
    int SeriesId,
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    int SeasonNumber,
    int EpisodesCount,
    string PosterUrl,
    double Rate);

public sealed record CreateSeasonCommand(SeasonData Data) : IRequest<SeasonDto>;

public sealed class CreateSeasonCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSeasonCommand, SeasonDto>
{
    public async Task<SeasonDto> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var series = await seriesRepository.GetWithSeasonsAsync(data.SeriesId, cancellationToken)
                     ?? throw new NotFoundException("Invalid Series ID");

        var season = series.AddSeason(
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            data.SeasonNumber,
            data.EpisodesCount);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return season.ToDto();
    }
}

public sealed record UpdateSeasonCommand(int Id, SeasonData Data) : IRequest<SeasonDto>;

public sealed class UpdateSeasonCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSeasonCommand, SeasonDto>
{
    public async Task<SeasonDto> Handle(UpdateSeasonCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var series = await seriesRepository.GetBySeasonIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Season not found");

        if (series.Id != data.SeriesId)
            throw new DomainException("A season cannot be moved to another series");

        var season = series.UpdateSeason(
            request.Id,
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            data.SeasonNumber,
            data.EpisodesCount);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return season.ToDto();
    }
}

public sealed record DeleteSeasonCommand(int Id) : IRequest;

public sealed class DeleteSeasonCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSeasonCommand>
{
    public async Task Handle(DeleteSeasonCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetBySeasonIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Season not found");

        var season = series.RemoveSeason(request.Id);

        seriesRepository.RemoveSeason(season);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
