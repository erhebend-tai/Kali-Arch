using MediatR;
using Microsoft.Extensions.Logging;

namespace CleanArch.Application.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken ct)
    {
        var requestName = typeof(TRequest).Name;

        logger.LogInformation("[START] {RequestName} {RequestData}",
            requestName, request);

        var response = await next();

        logger.LogInformation("[END] {RequestName}", requestName);
        return response;
    }
}
