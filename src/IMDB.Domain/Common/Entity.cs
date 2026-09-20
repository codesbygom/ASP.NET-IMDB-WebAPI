namespace IMDB.Domain.Common;

public abstract class Entity
{
    public int Id { get; protected set; }

    public override bool Equals(object? obj)
    {
        if (obj is not Entity other || other.GetType() != GetType())
            return false;

        if (ReferenceEquals(this, other))
            return true;

        return Id != 0 && Id == other.Id;
    }

    public override int GetHashCode() => Id == 0 ? base.GetHashCode() : HashCode.Combine(GetType(), Id);
}
