using IMDB.Application.Features.Account;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = await sender.Send(new RegisterCommand(request.Username, request.EmailAddress, request.Password));

        return ApiResponse.Success(user, "User registered successfully");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await sender.Send(new LoginCommand(request.UserName, request.Password));

        return ApiResponse.Success(user, "Login successful");
    }
}
