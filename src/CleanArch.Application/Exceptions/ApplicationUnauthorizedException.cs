namespace CleanArch.Application.Exceptions;

public sealed class ApplicationUnauthorizedException(string message)
    : Exception(message);
