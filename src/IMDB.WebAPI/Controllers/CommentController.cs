using IMDB.Application.Features.Comments;
using IMDB.WebAPI.Contracts;
using IMDB.WebAPI.Helpers;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommentController(ISender sender) : ControllerBase
{
    [HttpGet("media/{mediaId}")]
    public async Task<IActionResult> GetMediaComments([FromRoute] int mediaId)
    {
        return ApiResponse.Success(await sender.Send(new GetMediaCommentsQuery(mediaId)), "Comments retrieved successfully");
    }

    [Authorize]
    [HttpPost("media/{mediaId}")]
    public async Task<IActionResult> AddCommentToMedia([FromRoute] int mediaId, [FromBody] CommentRequest request)
    {
        return ApiResponse.Success(await sender.Send(new AddCommentCommand(mediaId, request.Text)), "Comment added successfully");
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateComment([FromRoute] int id, [FromBody] CommentRequest request)
    {
        return ApiResponse.Success(await sender.Send(new UpdateCommentCommand(id, request.Text)), "Comment updated successfully");
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteComment([FromRoute] int id)
    {
        await sender.Send(new DeleteCommentCommand(id));

        return ApiResponse.Success(true, "Comment deleted successfully");
    }
}
