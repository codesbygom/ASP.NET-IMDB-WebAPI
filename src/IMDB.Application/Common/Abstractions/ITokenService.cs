namespace IMDB.Application.Common.Abstractions;

public interface ITokenService
{
    string CreateToken(AuthenticatedUser user);
}
