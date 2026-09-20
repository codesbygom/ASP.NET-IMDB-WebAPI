using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Genres;

public sealed record CreateGenreCommand(string Title) : IRequest<GenreDto>;

public sealed class CreateGenreCommandHandler(IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<CreateGenreCommand, GenreDto>
{
    public async Task<GenreDto> Handle(CreateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = Genre.Create(request.Title);

        genres.Add(genre);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return genre.ToDto();
    }
}

public sealed record UpdateGenreCommand(int Id, string Title) : IRequest<GenreDto>;

public sealed class UpdateGenreCommandHandler(IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<UpdateGenreCommand, GenreDto>
{
    public async Task<GenreDto> Handle(UpdateGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genres.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Genre not found");

        genre.Rename(request.Title);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return genre.ToDto();
    }
}

public sealed record DeleteGenreCommand(int Id) : IRequest;

public sealed class DeleteGenreCommandHandler(IGenreRepository genres, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteGenreCommand>
{
    public async Task Handle(DeleteGenreCommand request, CancellationToken cancellationToken)
    {
        var genre = await genres.GetByIdAsync(request.Id, cancellationToken)
                    ?? throw new NotFoundException("Genre not found or could not be deleted");

        genres.Remove(genre);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
