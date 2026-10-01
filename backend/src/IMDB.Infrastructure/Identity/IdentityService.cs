using IMDB.Application.Common.Abstractions;
using IMDB.Application.Common.Constants;
using IMDB.Application.Common.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace IMDB.Infrastructure.Identity;

public sealed class IdentityService(UserManager<AppUser> userManager) : IIdentityService
{
    public async Task<string?> GetUserNameAsync(string userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);

        return user?.UserName;
    }

    public async Task<IReadOnlyDictionary<string, string?>> GetUserNamesAsync(
        IEnumerable<string> userIds,
        CancellationToken cancellationToken = default)
    {
        var ids = userIds.Distinct().ToList();

        if (ids.Count == 0)
            return new Dictionary<string, string?>();

        var users = await userManager.Users
            .Where(u => ids.Contains(u.Id))
            .Select(u => new { u.Id, u.UserName })
            .ToListAsync(cancellationToken);

        return users.ToDictionary(u => u.Id, u => u.UserName);
    }

    public async Task<AuthenticatedUser> RegisterAsync(
        string userName,
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = new AppUser { UserName = userName, Email = email };

        var created = await userManager.CreateAsync(user, password);
        if (!created.Succeeded)
            throw new IdentityOperationException(created.Errors.Select(e => e.Description));

        var roleAssigned = await userManager.AddToRoleAsync(user, Roles.User);
        if (!roleAssigned.Succeeded)
            throw new IdentityOperationException(roleAssigned.Errors.Select(e => e.Description));

        return await ToAuthenticatedUserAsync(user);
    }

    public async Task<AuthenticatedUser> ValidateCredentialsAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null || !await userManager.CheckPasswordAsync(user, password))
            throw new UnauthorizedException("Invalid credentials");

        return await ToAuthenticatedUserAsync(user);
    }

    private async Task<AuthenticatedUser> ToAuthenticatedUserAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);

        return new AuthenticatedUser(user.Id, user.UserName!, user.Email!, roles.ToList());
    }
}
