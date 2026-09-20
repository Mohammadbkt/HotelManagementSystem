using System.Collections.Concurrent;
using System.Reflection;
using FluentValidation;
using HotelManagementSystem.Domain.Common.Result;
using MediatR;

namespace HotelManagementSystem.Application.Common.Behaviors;

// TResponse has NO constraint here on purpose. We don't need the compiler to
// prove TResponse can build itself — we're going to ask it to, at runtime,
// via reflection. That's what lets this work for Result AND Result<TValue>
// without touching Result.cs and without a shared interface between them.
public class ValidationBehaviors<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    // Reflection (GetMethod) is relatively expensive. We only want to pay that
    // cost ONCE per response type (Result<Guid>, Result<InvoiceDto>, ...),
    // not once per request. Cache the MethodInfo itself, keyed by TResponse.
    private static readonly ConcurrentDictionary<Type, MethodInfo> _failureMethodCache = new();

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Run every validator registered for this TRequest (FluentValidation
        // supports multiple validators per command) in parallel.
        var results = await Task.WhenAll(validators.Select(v => v.ValidateAsync(request, cancellationToken)));

        // Flatten every validator's errors into your domain's Error type —
        // same Error.Validation(...) shape your handlers already use.
        var errors = results.SelectMany(r => r.Errors)
            .Select(e => Error.Validation(e.PropertyName, e.ErrorMessage))
            .ToList();

        // Happy path: nothing failed, so we don't even need to know what
        // TResponse is — just hand off to the actual handler.
        if (errors.Count == 0)
            return await next(cancellationToken);

        // Unhappy path: we need to return a TResponse, but the compiler can't
        // build one for us. So we go find TResponse's own `Failure` method
        // at runtime and call it ourselves — this is the one line doing the
        // "impossible" thing every previous attempt couldn't get the
        // compiler to agree to.
        var failureMethod = _failureMethodCache.GetOrAdd(typeof(TResponse), t =>
            t.GetMethod("Failure", BindingFlags.Public | BindingFlags.Static, [typeof(IReadOnlyList<Error>)])
            ?? throw new InvalidOperationException(
                $"{t.Name} has no static Failure(IReadOnlyList<Error>) method."));

        // Invoke(null, ...) — null because Failure is static, there's no
        // instance to call it on. Cast object -> TResponse: safe because at
        // runtime the object really IS a Result or Result<Guid> etc.
        return (TResponse)failureMethod.Invoke(null, [errors])!;
    }
}