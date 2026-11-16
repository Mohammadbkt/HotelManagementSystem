using System.Diagnostics;
using HotelManagementSystem.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace HotelManagementSystem.Application.Common.Behaviors;

public sealed class LoggingBehavior<TRequest, TResponse>(
    ILogger<TRequest> logger,
    ICurrentUserService currentUser)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<TRequest> _logger = logger;
    private readonly ICurrentUserService _currentUser = currentUser;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var userId = _currentUser.UserId;

        var started = Stopwatch.GetTimestamp();

        try
        {
            return await next(cancellationToken);
        }
        finally
        {
            var elapsed = Stopwatch.GetElapsedTime(started).TotalMilliseconds;

            _logger.LogInformation(
                "Handled {RequestName} in {ElapsedMs:F2} ms by user {UserId}",
                requestName, elapsed, userId);
        }
    }
}