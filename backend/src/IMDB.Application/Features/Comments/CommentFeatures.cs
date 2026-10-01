using IMDB.Application.Common.Abstractions;
using IMDB.Application.Common.Exceptions;
using IMDB.Application.DTOs;
using IMDB.Application.Mappings;
using IMDB.Domain.Common;
using IMDB.Domain.Entities;
using IMDB.Domain.Repositories;
using MediatR;

namespace IMDB.Application.Features.Comments;

public sealed record GetMediaCommentsQuery(int MediaId) : IRequest<IReadOnlyList<CommentDto>>;

public sealed class GetMediaCommentsQueryHandler(ICommentRepository comments, IIdentityService identity)
    : IRequestHandler<GetMediaCommentsQuery, IReadOnlyList<CommentDto>>
{
    public async Task<IReadOnlyList<CommentDto>> Handle(GetMediaCommentsQuery request, CancellationToken cancellationToken)
    {
        var items = await comments.GetByMediaIdAsync(request.MediaId, cancellationToken);
        var names = await identity.GetUserNamesAsync(items.Select(c => c.UserId), cancellationToken);

        return items.Select(c => c.ToDto(names.GetValueOrDefault(c.UserId))).ToList();
    }
}

public sealed record AddCommentCommand(int MediaId, string Text) : IRequest<CommentDto>;

public sealed class AddCommentCommandHandler(
    ICommentRepository comments,
    IMediaRepository media,
    IIdentityService identity,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<AddCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        if (!await media.ExistsAsync(request.MediaId, cancellationToken))
            throw new NotFoundException("Media not found");

        var comment = Comment.Create(request.MediaId, userId, request.Text);

        comments.Add(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.ToDto(await identity.GetUserNameAsync(userId, cancellationToken));
    }
}

public sealed record UpdateCommentCommand(int Id, string Text) : IRequest<CommentDto>;

public sealed class UpdateCommentCommandHandler(
    ICommentRepository comments,
    IIdentityService identity,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<UpdateCommentCommand, CommentDto>
{
    public async Task<CommentDto> Handle(UpdateCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        var comment = await comments.GetByIdAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException("Comment not found");

        comment.Edit(userId, request.Text);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return comment.ToDto(await identity.GetUserNameAsync(userId, cancellationToken));
    }
}

public sealed record DeleteCommentCommand(int Id) : IRequest;

public sealed class DeleteCommentCommandHandler(
    ICommentRepository comments,
    ICurrentUser currentUser,
    IUnitOfWork unitOfWork) : IRequestHandler<DeleteCommentCommand>
{
    public async Task Handle(DeleteCommentCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.GetRequiredUserId();

        var comment = await comments.GetByIdAsync(request.Id, cancellationToken)
                      ?? throw new NotFoundException("Comment not found");

        comment.EnsureOwnedBy(userId);

        comments.Remove(comment);
        await unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
