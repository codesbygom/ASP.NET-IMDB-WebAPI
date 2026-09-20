using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Features.Genres;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Series;

public sealed record SeriesData(
    string ImdbId,
    string Title,
    DateTime ReleaseDate,
    string Description,
    string PosterUrl,
    double Rate,
    IReadOnlyList<int> GenreIds);

public sealed record CreateSeriesCommand(SeriesData Data) : IRequest<SeriesDto>;

public sealed class CreateSeriesCommandHandler(ISeriesRepository seriesRepository, IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateSeriesCommand, SeriesDto>
{
    public async Task<SeriesDto> Handle(CreateSeriesCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;
        var seriesGenres = await GenreLookup.ResolveAsync(genres, data.GenreIds, cancellationToken);

        var series = Domain.Entities.Series.Create(
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            seriesGenres);

        seriesRepository.Add(series);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return series.ToDto();
    }
}

public sealed record UpdateSeriesCommand(int Id, SeriesData Data) : IRequest<SeriesDto>;

public sealed class UpdateSeriesCommandHandler(ISeriesRepository seriesRepository, IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateSeriesCommand, SeriesDto>
{
    public async Task<SeriesDto> Handle(UpdateSeriesCommand request, CancellationToken cancellationToken)
    {
        var data = request.Data;

        var series = await seriesRepository.GetByIdAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Series not found");

        var seriesGenres = await GenreLookup.ResolveAsync(genres, data.GenreIds, cancellationToken);

        series.Update(
            data.ImdbId,
            data.Title,
            data.ReleaseDate,
            data.Description,
            data.PosterUrl,
            data.Rate,
            seriesGenres);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return series.ToDto();
    }
}

public sealed record DeleteSeriesCommand(int Id) : IRequest;

public sealed class DeleteSeriesCommandHandler(ISeriesRepository seriesRepository, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteSeriesCommand>
{
    public async Task Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesRepository.GetWithSeasonsAsync(request.Id, cancellationToken)
                     ?? throw new NotFoundException("Series not found");

        seriesRepository.Remove(series);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
