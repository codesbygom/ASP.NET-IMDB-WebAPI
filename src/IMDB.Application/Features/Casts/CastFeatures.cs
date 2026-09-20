using IMDB.Application.Common.Exceptions;
using IMDB.Application.Common.Models;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Enums;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Casts;

public sealed record GetCastsQuery(int? Page, int? PageSize) : IRequest<PagedResult<CastDto>>;

public sealed class GetCastsQueryHandler(ICastRepository casts)
    : IRequestHandler<GetCastsQuery, PagedResult<CastDto>>
{
    public Task<PagedResult<CastDto>> Handle(GetCastsQuery request, CancellationToken cancellationToken)
    {
        return casts.GetResultAsync(request.Page, request.PageSize, c => c.ToDto(), cancellationToken);
    }
}

public sealed record GetMediaCastQuery(int MediaId, int? Page, int? PageSize) : IRequest<PagedResult<CastDto>>;

public sealed class GetMediaCastQueryHandler(ICastRepository casts)
    : IRequestHandler<GetMediaCastQuery, PagedResult<CastDto>>
{
    public async Task<PagedResult<CastDto>> Handle(GetMediaCastQuery request, CancellationToken cancellationToken)
    {
        if (request.Page.HasValue && request.PageSize.HasValue)
        {
            var (page, pageSize) = PagingExtensions.Normalize(request.Page.Value, request.PageSize.Value);
            var (items, total) = await casts.GetPagedByMediaIdAsync(request.MediaId, page, pageSize, cancellationToken);

            return PagedResult<CastDto>.Paged(items.Select(c => c.ToDto()).ToList(), total, page, pageSize);
        }

        var all = await casts.GetByMediaIdAsync(request.MediaId, cancellationToken);

        return PagedResult<CastDto>.All(all.Select(c => c.ToDto()).ToList());
    }
}

public sealed record AddCastCommand(int MediaId, int PersonId, CastRole Role) : IRequest<CastDto>;

public sealed class AddCastCommandHandler(
    ICastRepository casts,
    IMediaRepository media,
    IPersonRepository people,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCastCommand, CastDto>
{
    public async Task<CastDto> Handle(AddCastCommand request, CancellationToken cancellationToken)
    {
        if (!await media.ExistsAsync(request.MediaId, cancellationToken))
            throw new NotFoundException("Media not found");

        var person = await people.GetByIdAsync(request.PersonId, cancellationToken)
                     ?? throw new NotFoundException("Person not found");

        var cast = Cast.Create(request.MediaId, request.PersonId, request.Role);

        casts.Add(cast);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return cast.ToDto(person);
    }
}

public sealed record UpdateCastCommand(int Id, int MediaId, int PersonId, CastRole Role) : IRequest<CastDto>;

public sealed class UpdateCastCommandHandler(
    ICastRepository casts,
    IMediaRepository media,
    IPersonRepository people,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCastCommand, CastDto>
{
    public async Task<CastDto> Handle(UpdateCastCommand request, CancellationToken cancellationToken)
    {
        var cast = await casts.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new NotFoundException("Cast not found");

        if (!await media.ExistsAsync(request.MediaId, cancellationToken))
            throw new NotFoundException("Media not found");

        var person = await people.GetByIdAsync(request.PersonId, cancellationToken)
                     ?? throw new NotFoundException("Person not found");

        cast.Update(request.MediaId, request.PersonId, request.Role);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return cast.ToDto(person);
    }
}

public sealed record DeleteCastCommand(int Id) : IRequest;

public sealed class DeleteCastCommandHandler(ICastRepository casts, IUnitOfWork unitOfWork)
    : IRequestHandler<DeleteCastCommand>
{
    public async Task Handle(DeleteCastCommand request, CancellationToken cancellationToken)
    {
        var cast = await casts.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new NotFoundException("Cast not found");

        casts.Remove(cast);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
