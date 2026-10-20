namespace CleanArch.Shared.Abstractions.Domain;

/// <summary>
/// Domain business rule - checked inside entities and factories;
/// a broken rule raises a domain exception.
/// </summary>
public interface IBusinessRule
{
    bool IsBroken();
    string Message { get; }
}
