using IMDB.Application.Common.Exceptions;
using IMDB.Domain.Common;

namespace IMDB.WebAPI.Middleware;

public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception exception)
        {
            var (statusCode, message, errors) = Map(exception);

            if (statusCode == StatusCodes.Status500InternalServerError)
                logger.LogError(exception, "An unhandled exception occurred");

            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                success = false,
                message,
                errors,
                timestamp = DateTime.UtcNow
            });
        }
    }

    private static (int StatusCode, string Message, IReadOnlyList<string>? Errors) Map(Exception exception)
    {
        return exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, exception.Message, null),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, exception.Message, null),
            ForbiddenDomainException => (StatusCodes.Status403Forbidden, exception.Message, null),
            DomainException => (StatusCodes.Status400BadRequest, exception.Message, null),
            IdentityOperationException identity => (StatusCodes.Status400BadRequest, "Request could not be completed", identity.Errors),
            _ => (StatusCodes.Status500InternalServerError, "An error occurred while processing your request", null)
        };
    }
}
