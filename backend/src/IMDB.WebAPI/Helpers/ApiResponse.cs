using IMDB.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace IMDB.WebAPI.Helpers;

public static class ApiResponse
{
    public static IActionResult Success<T>(T data, string message = "Success")
    {
        return new OkObjectResult(new
        {
            success = true,
            message,
            data,
            timestamp = DateTime.UtcNow
        });
    }

    public static IActionResult Paged<T>(PagedResult<T> result, string message, string pagedMessage)
    {
        if (!result.IsPaged)
            return Success(result.Items, message);

        return Success(new
        {
            result.TotalCount,
            Page = result.Page!.Value,
            PageSize = result.PageSize!.Value,
            Data = result.Items
        }, pagedMessage);
    }

    public static IActionResult Error(string message, int statusCode, IEnumerable<string>? errors = null)
    {
        return new ObjectResult(new
        {
            success = false,
            message,
            errors,
            timestamp = DateTime.UtcNow
        })
        {
            StatusCode = statusCode
        };
    }
}
