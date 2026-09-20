using IMDB.Domain.Common;
using IMDB.Domain.ValueObjects;

namespace IMDB.Domain.Entities;

public sealed class Person : Entity, IAggregateRoot
{
    private Person()
    {
        ImdbId = null!;
        FullName = null!;
        Bio = null!;
        PhotoUrl = null!;
    }

    public ImdbId ImdbId { get; private set; }
    public string FullName { get; private set; }
    public DateTime BirthDate { get; private set; }
    public string Bio { get; private set; }
    public string PhotoUrl { get; private set; }

    public static Person Create(string imdbId, string fullName, DateTime birthDate, string bio, string photoUrl)
    {
        var person = new Person();
        person.Apply(imdbId, fullName, birthDate, bio, photoUrl);
        return person;
    }

    public void Update(string imdbId, string fullName, DateTime birthDate, string bio, string photoUrl)
    {
        Apply(imdbId, fullName, birthDate, bio, photoUrl);
    }

    private void Apply(string imdbId, string fullName, DateTime birthDate, string bio, string photoUrl)
    {
        var name = Guard.NotEmpty(fullName, "Full name", 100);

        if (name.Length < 2)
            throw new DomainException("Full name must be at least 2 characters");

        if (birthDate.Date > DateTime.UtcNow.Date)
            throw new DomainException("Birth date cannot be in the future");

        ImdbId = ImdbId.From(imdbId);
        FullName = name;
        BirthDate = birthDate;
        Bio = Guard.NotEmpty(bio, "Biography", 2000);
        PhotoUrl = Guard.Url(photoUrl, "Photo URL");
    }
}
