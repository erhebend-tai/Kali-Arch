namespace CleanArch.Shared.RegularExpressions.Validators;

public static class EmailValidator
{
    public static bool IsValid(string value) =>
        Patterns.Email().IsMatch(value);
}
