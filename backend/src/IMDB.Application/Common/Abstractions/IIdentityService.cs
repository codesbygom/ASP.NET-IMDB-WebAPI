namespace IMDB.Application.Common.Abstractions;

public sealed record AuthenticatedUser(string Id, string UserName, string Email, IReadOnlyList<string> Roles);

public interface IIdentityService
{
    Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<string, string?>> GetUserNamesAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default);

    Task<AuthenticatedUser> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken = default);

    Task<AuthenticatedUser> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default);
}
