using CleanArch.Shared.RegularExpressions;
using FluentValidation;

namespace CleanArch.Application.Validators.Common;

/// <summary>
/// Reusable rule — any command property carrying a resource name.
/// </summary>
public static class ResourceNameValidator
{
    public static IRuleBuilderOptions<T, string> IsResourceName<T>(
        this IRuleBuilder<T, string> rule) =>
        rule.Must(v => Patterns.ResourceName().IsMatch(v))
            .WithErrorCode("Resource.Name.InvalidFormat")
            .WithMessage(
                "Must be a valid resource name (lowercase " +
                "alphanumeric + hyphens, max 63 chars).");
}
