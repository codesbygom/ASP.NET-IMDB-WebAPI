using IMDB.Application.Common.Constants;
using IMDB.Application.Features.Episodes;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EpisodeController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return ApiResponse.Success(await sender.Send(new GetEpisodesQuery()), "Episodes retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetEpisodeQuery(id)), "Episode retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] EpisodeRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreateEpisodeCommand(request.ToData())), "Episode created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] EpisodeRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateEpisodeCommand(id, request.ToData())), "Episode updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeleteEpisodeCommand(id));

        return ApiResponse.Success(true, "Episode has been deleted");
    }
}
