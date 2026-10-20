namespace CleanArch.Application.Exceptions;

/// <summary>
/// Application-layer exception — mapped to 404 by
/// ExceptionHandlingMiddleware. Domain exceptions stay in Domain.
/// </summary>
public sealed class NotFoundException(string message)
    : Exception(message);
