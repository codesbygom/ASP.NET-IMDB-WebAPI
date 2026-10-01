using IMDB.Domain.Common;
using IMDB.Domain.Enums;

namespace IMDB.Domain.Entities;

public sealed class Rate : Entity, IAggregateRoot
{
    private Rate()
    {
        UserId = null!;
    }

    public int MediaId { get; private set; }
    public string UserId { get; private set; }
    public ScoreEnum Score { get; private set; }
    public DateTime CreatedOn { get; private set; }
    public DateTime LastUpdatedOn { get; private set; }

    public static Rate Create(int mediaId, string userId, ScoreEnum score)
    {
        if (mediaId <= 0)
            throw new DomainException("Invalid media ID");

        var now = DateTime.UtcNow;

        return new Rate
        {
            MediaId = mediaId,
            UserId = Guard.NotEmpty(userId, "User", 450),
            Score = ValidateScore(score),
            CreatedOn = now,
            LastUpdatedOn = now
        };
    }

    public void ChangeScore(string userId, ScoreEnum score)
    {
        EnsureOwnedBy(userId);

        Score = ValidateScore(score);
        LastUpdatedOn = DateTime.UtcNow;
    }

    public void EnsureOwnedBy(string userId)
    {
        if (UserId != userId)
            throw new ForbiddenDomainException("Only the author can modify this rate");
    }

    private static ScoreEnum ValidateScore(ScoreEnum score)
    {
        if (!Enum.IsDefined(score))
            throw new DomainException("Score must be between 0 and 5");

        return score;
    }
}
