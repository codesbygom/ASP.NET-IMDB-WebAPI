using IMDB.Domain.Common;
using IMDB.Domain.Enums;

namespace IMDB.Domain.Entities;

public sealed class Cast : Entity, IAggregateRoot
{
    private Cast()
    {
    }

    public int MediaId { get; private set; }
    public int PersonId { get; private set; }
    public CastRole Role { get; private set; }
    public Person? Person { get; private set; }

    public static Cast Create(int mediaId, int personId, CastRole role)
    {
        var cast = new Cast();
        cast.Apply(mediaId, personId, role);
        return cast;
    }

    public void Update(int mediaId, int personId, CastRole role)
    {
        Apply(mediaId, personId, role);
    }

    private void Apply(int mediaId, int personId, CastRole role)
    {
        if (mediaId <= 0)
            throw new DomainException("Invalid media ID");

        if (personId <= 0)
            throw new DomainException("Invalid person ID");

        if (!Enum.IsDefined(role))
            throw new DomainException("Invalid cast role");

        if (PersonId != personId)
            Person = null;

        MediaId = mediaId;
        PersonId = personId;
        Role = role;
    }
}
