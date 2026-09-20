using IMDB.Application.Features.Rates;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RateController(ISender sender) : ControllerBase
{
    [HttpGet("media/{mediaId}")]
    public async Task<IActionResult> GetMediaRates([FromRoute] int mediaId)
    {
        return ApiResponse.Success(await sender.Send(new GetMediaRatesQuery(mediaId)), "Rates retrieved successfully");
    }

    [Authorize]
    [HttpPost("media/{mediaId}")]
    public async Task<IActionResult> AddMediaRate([FromRoute] int mediaId, [FromBody] RateRequest request)
    {
        return ApiResponse.Success(await sender.Send(new AddRateCommand(mediaId, request.Score)), "Rate added successfully");
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateRate([FromRoute] int id, [FromBody] RateRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateRateCommand(id, request.Score)), "Rate updated successfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteRate([FromRoute] int id)
    {
        await sender.Send(new DeleteRateCommand(id));

        return ApiResponse.Success(true, "Rate deleted successfully");
    }
}
