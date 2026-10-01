using IMDB.Application.Features.Casts;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CastController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCasts([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var result = await sender.Send(new GetCastsQuery(page, pageSize));

        return ApiResponse.Paged(result, "All casts retrieved successfully", "Paginated casts retrieved successfully");
    }

    [HttpGet("media/{mediaId}")]
    public async Task<IActionResult> GetMediaCast([FromRoute] int mediaId, [FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var result = await sender.Send(new GetMediaCastQuery(mediaId, page, pageSize));

        return ApiResponse.Paged(result, "Media casts retrieved successfully", "Paginated media casts retrieved successfully");
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddMediaCast([FromBody] CastRequest request)
    {
        var cast = await sender.Send(new AddCastCommand(request.MediaId, request.PersonId, request.Role));

        return ApiResponse.Success(cast, "Cast added successfully");
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCast([FromRoute] int id, [FromBody] CastRequest request)
    {
        var cast = await sender.Send(new UpdateCastCommand(id, request.MediaId, request.PersonId, request.Role));

        return ApiResponse.Success(cast, "Cast updated successfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCast([FromRoute] int id)
    {
        await sender.Send(new DeleteCastCommand(id));

        return ApiResponse.Success(true, "Cast deleted successfully");
    }
}
