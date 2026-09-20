namespace IMDB.Domain.Common;

public static class Guard
{
    public static string NotEmpty(string? value, string name, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new DomainException($"{name} is required");

        var trimmed = value.Trim();

        if (trimmed.Length > maxLength)
            throw new DomainException($"{name} must not exceed {maxLength} characters");

        return trimmed;
    }

    public static int InRange(int value, int min, int max, string name)
    {
        if (value < min || value > max)
            throw new DomainException($"{name} must be between {min} and {max}");

        return value;
    }

    public static string Url(string? value, string name, int maxLength = 500)
    {
        var trimmed = NotEmpty(value, name, maxLength);

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new DomainException($"{name} must be a valid URL");

        return trimmed;
    }
}
