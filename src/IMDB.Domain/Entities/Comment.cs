using IMDB.Domain.Common;

namespace IMDB.Domain.Entities;

public sealed class Comment : Entity, IAggregateRoot
{
    private Comment()
    {
        Text = null!;
        UserId = null!;
    }

    public string Text { get; private set; }
    public int MediaId { get; private set; }
    public string UserId { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime LastUpdatedOn { get; private set; }

    public static Comment Create(int mediaId, string userId, string text)
    {
        if (mediaId <= 0)
            throw new DomainException("Invalid media ID");

        var now = DateTime.UtcNow;

        return new Comment
        {
            MediaId = mediaId,
            UserId = Guard.NotEmpty(userId, "User", 450),
            Text = Guard.NotEmpty(text, "Comment text", 1000),
            CreatedOn = now,
            LastUpdatedOn = now
        };
    }

    public void Edit(string userId, string text)
    {
        EnsureOwnedBy(userId);

        Text = Guard.NotEmpty(text, "Comment text", 1000);
        LastUpdatedOn = DateTime.UtcNow;
    }

    public void EnsureOwnedBy(string userId)
    {
        if (UserId != userId)
            throw new ForbiddenDomainException("Only the author can modify this comment");
    }
}
