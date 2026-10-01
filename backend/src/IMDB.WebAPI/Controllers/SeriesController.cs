using IMDB.Application.Common.Constants;
using IMDB.Application.Features.Series;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SeriesController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        return ApiResponse.Success(await sender.Send(new GetSeriesListQuery()), "Series retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetSeriesQuery(id)), "Series retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SeriesRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreateSeriesCommand(request.ToData())), "Series created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] SeriesRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateSeriesCommand(id, request.ToData())), "Series updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeleteSeriesCommand(id));

        return ApiResponse.Success(true, "Series has been deleted");
    }
}
