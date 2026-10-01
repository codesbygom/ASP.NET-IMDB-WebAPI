using IMDB.Domain.Common;

namespace IMDB.Domain.Entities;

public sealed class Genre : Entity, IAggregateRoot
{
    private Genre()
    {
        Title = null!;
    }

    public string Title { get; private set; }

    public static Genre Create(string title)
    {
        return new Genre { Title = ValidateTitle(title) };
    }

    public void Rename(string title)
    {
        Title = ValidateTitle(title);
    }

    private static string ValidateTitle(string title)
    {
        var value = Guard.NotEmpty(title, "Genre title", 50);

        if (value.Length < 2)
            throw new DomainException("Genre title must be at least 2 characters");

        return value;
    }
}
