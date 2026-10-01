using IMDB.Application.Common.Constants;
using IMDB.Application.Features.People;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PersonController(ISender sender) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] int? page, [FromQuery] int? pageSize)
    {
        var result = await sender.Send(new GetPeopleQuery(page, pageSize));

        return ApiResponse.Paged(result, "People retrieved successfully", "Paginated people retrieved successfully");
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDetail([FromRoute] int id)
    {
        return ApiResponse.Success(await sender.Send(new GetPersonQuery(id)), "Person retrieved successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] PersonRequest request)
    {
        return ApiResponse.Success(await sender.Send(new CreatePersonCommand(request.ToData())), "Person created successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] PersonRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdatePersonCommand(id, request.ToData())), "Person updated successfully");
    }

    [Authorize(Roles = Roles.Admin)]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        await sender.Send(new DeletePersonCommand(id));

        return ApiResponse.Success(true, "Person has been deleted successfully");
    }
}
