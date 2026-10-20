namespace CleanArch.Shared.Results;

/// <summary>
/// Carries multiple validation failures — ValidationBehavior
/// aggregates these into one Error.
/// </summary>
public sealed record ValidationError(
    string PropertyName,
    string ErrorCode,
    string ErrorMessage);
