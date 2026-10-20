namespace CleanArch.Shared.Results;

public enum ErrorType
{
    None,
    Failure,
    Validation,
    NotFound,
    Conflict,
    Unauthorized,
    Forbidden,
    Domain,
    Unexpected
}
