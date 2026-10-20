namespace CleanArch.Domain.Exceptions;

/// <summary>
/// Thrown when a domain invariant is violated. Mapped to
/// ProblemDetails / a failed Result at the application boundary.
/// </summary>
public abstract class DomainException(string message) : Exception(message);
