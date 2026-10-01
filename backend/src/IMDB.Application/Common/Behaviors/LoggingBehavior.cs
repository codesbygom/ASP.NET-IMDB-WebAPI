using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace IMDB.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var name = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var response = await next();
            logger.LogInformation("Handled {Request} in {Elapsed} ms", name, stopwatch.ElapsedMilliseconds);
            return response;
        }
        catch (Exception exception)
        {
            logger.LogWarning("Request {Request} failed after {Elapsed} ms: {Message}", name, stopwatch.ElapsedMilliseconds, exception.Message);
            throw;
        }
    }
}
