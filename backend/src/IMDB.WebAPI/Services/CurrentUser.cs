using System.Security.Claims;
using IMDB.Application.Common.Abstractions;

namespace IMDB.WebAPI.Services;

public sealed class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public string? UserId => httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
}
