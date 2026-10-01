using IMDB.Domain.Common;

namespace IMDB.Domain.ValueObjects;

public sealed record ImdbId
{
    public const int MaxLength = 20;

    public string Value { get; }

    private ImdbId(string value)
    {
        Value = value;
    }

    public static ImdbId From(string? value)
    {
        return new ImdbId(Guard.NotEmpty(value, "IMDB ID", MaxLength));
    }
}
