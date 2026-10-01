namespace IMDB.Application.Common.Exceptions;

public class NotFoundException(string message) : Exception(message);

public class UnauthorizedException(string message) : Exception(message);

public class IdentityOperationException(IEnumerable<string> errors) : Exception(string.Join("; ", errors))
{
    public IReadOnlyList<string> Errors { get; } = errors.ToList();
}
