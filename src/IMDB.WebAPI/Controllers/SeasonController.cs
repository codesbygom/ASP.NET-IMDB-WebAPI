using IMDB.Application.Common.Constants;
using IMDB.Application.Features.Seasons;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeasonController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return ApiResponse.Success(await sender.Send(new GetSeasonsQuery()), "Seasons retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetSeasonQuery(id)), "Season retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SeasonRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreateSeasonCommand(request.ToData())), "Season created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SeasonRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateSeasonCommand(id, request.ToData())), "Season updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeleteSeasonCommand(id));

        return ApiResponse.Success(true, "Season has been deleted");
    }
}
