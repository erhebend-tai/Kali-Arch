namespace CleanArch.Shared.RegularExpressions.Validators;

public static class ResourceNameValidator
{
    public static bool IsValid(string value) =>
        Patterns.ResourceName().IsMatch(value);
}
