using IMDB.Application.Common.Constants;
using IMDB.Application.Features.Movies;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class MovieController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return ApiResponse.Success(await sender.Send(new GetMoviesQuery()), "Movies retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetMovieQuery(id)), "Movie retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] MovieRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreateMovieCommand(request.ToData())), "Movie created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] MovieRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateMovieCommand(id, request.ToData())), "Movie updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeleteMovieCommand(id));

        return ApiResponse.Success(true, "Movie has been deleted");
    }
}
