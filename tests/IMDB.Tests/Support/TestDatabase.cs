using IMDB.Application.Common.Abstractions;
using IMDB.Infrastructure.Persistence;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Tests.Support;

public sealed class TestDatabase : IDisposable
{
    private readonly SqliteConnection _connection = new("DataSource=:memory:");

    public TestDatabase()
    {
        _connection.Open();

        var options = new DbContextOptionsBuilder<ApplicationDbContext>().UseSqlite(_connection).Options;

        Context = new ApplicationDbContext(options);
        Context.Database.EnsureCreated();
    }

    public ApplicationDbContext Context { get; }

    public void Dispose()
    {
        Context.Dispose();
        _connection.Dispose();
    }
}

public sealed class FakeCurrentUser(string? userId) : ICurrentUser
{
    public string? UserId { get; } = userId;
}

public sealed class FakeIdentityService : IIdentityService
{
    public Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
        => Task.FromResult<string?>($"name-of-{userId}");

    public Task<IReadOnlyDictionary<string, string?>> GetUserNamesAsync(IEnumerable<string> userIds, CancellationToken cancellationToken = default)
    {
        IReadOnlyDictionary<string, string?> names = userIds.Distinct().ToDictionary(id => id, id => (string?)$"name-of-{id}");
        return Task.FromResult(names);
    }

    public Task<AuthenticatedUser> RegisterAsync(string userName, string email, string password, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();

    public Task<AuthenticatedUser> ValidateCredentialsAsync(string userName, string password, CancellationToken cancellationToken = default)
        => throw new NotSupportedException();
}
