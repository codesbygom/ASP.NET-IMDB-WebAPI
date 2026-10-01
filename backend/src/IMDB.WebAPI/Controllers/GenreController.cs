using IMDB.Application.Common.Constants;
using IMDB.Application.Features.Genres;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GenreController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var result = await sender.Send(new GetGenresQuery(page, pageSize));

        return ApiResponse.Paged(result, "Genres retrieved successfully", "Paginated genres retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetGenreQuery(id)), "Genre retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] GenreRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreateGenreCommand(request.Title)), "Genre created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] GenreRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateGenreCommand(id, request.Title)), "Genre updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeleteGenreCommand(id));

        return ApiResponse.Success(true, "Genre has been deleted successfully");
    }
}
