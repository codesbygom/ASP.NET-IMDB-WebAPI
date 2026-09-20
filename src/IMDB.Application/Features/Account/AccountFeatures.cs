using IMDB.Application.Common.Abstractions;
using IMDB.Application.DTOs;
using MediatR;

namespace IMDB.Application.Features.Account;

public sealed record RegisterCommand(string UserName, string Email, string Password) : IRequest<NewUserDto>;

public sealed class RegisterCommandHandler(IIdentityService identity, ITokenService tokens)
    : IRequestHandler<RegisterCommand, NewUserDto>
{
    public async Task<NewUserDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var user = await identity.RegisterAsync(request.UserName, request.Email, request.Password, cancellationToken);

        return new NewUserDto(user.UserName, user.Email, tokens.CreateToken(user));
    }
}

public sealed record LoginCommand(string UserName, string Password) : IRequest<NewUserDto>;

public sealed class LoginCommandHandler(IIdentityService identity, ITokenService tokens)
    : IRequestHandler<LoginCommand, NewUserDto>
{
    public async Task<NewUserDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await identity.ValidateCredentialsAsync(request.UserName, request.Password, cancellationToken);

        return new NewUserDto(user.UserName, user.Email, tokens.CreateToken(user));
    }
}
