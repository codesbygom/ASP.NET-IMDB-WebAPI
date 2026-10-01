using IMDB.Domain.Common;

namespace IMDB.Domain.ValueObjects;

public sealed record Rating
{
    public double Value { get; }

    private Rating(double value)
    {
        Value = value;
    }

    public static Rating Zero { get; } = new(0);

    public static Rating From(double value)
    {
        if (double.IsNaN(value) || value < 0 || value > 10)
            throw new DomainException("Rating must be between 0 and 10");

        return new Rating(Math.Round(value, 1));
    }
}
