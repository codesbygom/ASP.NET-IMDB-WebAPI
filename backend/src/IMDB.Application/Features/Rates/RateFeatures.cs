using IMDB.Application.Common.Abstractions;
using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Enums;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Rates;

public sealed record GetMediaRatesQuery(int MediaId) : IRequest<IReadOnlyList<RateDto>>;

public sealed class GetMediaRatesQueryHandler(IRateRepository rates, IIdentityService identity)
    : IRequestHandler<GetMediaRatesQuery, IReadOnlyList<RateDto>>
{
    public async Task<IReadOnlyList<RateDto>> Handle(GetMediaRatesQuery request, CancellationToken cancellationToken)
    {
        var items = await rates.GetByMediaIdAsync(request.MediaId, cancellationToken);
        var names = await identity.GetUserNamesAsync(items.Select(r => r.UserId), cancellationToken);

        return items.Select(r => r.ToDto(names.GetValueOrDefault(r.UserId))).ToList();
    }
}

public sealed record AddRateCommand(int MediaId, ScoreEnum Score) : IRequest<RateDto>;

public sealed class AddRateCommandHandler(
    IRateRepository rates,
    IMediaRepository media,
    IIdentityService identity,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<AddRateCommand, RateDto>
{
    public async Task<RateDto> Handle(AddRateCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        if (!await media.ExistsAsync(request.MediaId, cancellationToken))
            throw new NotFoundException("Media not found");

        var rate = Rate.Create(request.MediaId, userId, request.Score);

        rates.Add(rate);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return rate.ToDto(await identity.GetUserNameAsync(userId, cancellationToken));
    }
}

public sealed record UpdateRateCommand(int Id, ScoreEnum Score) : IRequest<RateDto>;

public sealed class UpdateRateCommandHandler(
    IRateRepository rates,
    IIdentityService identity,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateRateCommand, RateDto>
{
    public async Task<RateDto> Handle(UpdateRateCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        var rate = await rates.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new NotFoundException("Rate not found");

        rate.ChangeScore(userId, request.Score);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return rate.ToDto(await identity.GetUserNameAsync(userId, cancellationToken));
    }
}

public sealed record DeleteRateCommand(int Id) : IRequest;

public sealed class DeleteRateCommandHandler(
    IRateRepository rates,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteRateCommand>
{
    public async Task Handle(DeleteRateCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        var rate = await rates.GetByIdAsync(request.Id, cancellationToken)
                   ?? throw new NotFoundException("Rate not found");

        rate.EnsureOwnedBy(userId);

        rates.Remove(rate);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
