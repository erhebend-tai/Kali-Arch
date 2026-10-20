using CleanArch.Application.Commands.Compute;
using CleanArch.Shared.RegularExpressions;
using FluentValidation;

namespace CleanArch.Application.Validators.Compute;

public sealed class ProvisionVmCommandValidator
    : AbstractValidator<ProvisionVmCommand>
{
    private static readonly string[] ValidRegions =
        ["us-east", "us-west", "europe-west", "europe-north",
         "asia-east", "asia-southeast"];

    private static readonly string[] ValidSizes =
        ["small", "medium", "large", "xlarge", "xxlarge"];

    public ProvisionVmCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
                .WithErrorCode("Vm.Name.Required")
            .MinimumLength(2)
                .WithErrorCode("Vm.Name.TooShort")
            .MaximumLength(63)
                .WithErrorCode("Vm.Name.TooLong")
            .Must(n => Patterns.ResourceName().IsMatch(n))
                .WithErrorCode("Vm.Name.InvalidFormat")
                .WithMessage(
                    "VM name may only contain lowercase letters, " +
                    "numbers, and hyphens.");

        RuleFor(x => x.Region)
            .NotEmpty()
                .WithErrorCode("Vm.Region.Required")
            .Must(r => ValidRegions.Contains(r))
                .WithErrorCode("Vm.Region.Invalid")
                .WithMessage(x =>
                    $"'{x.Region}' is not a valid region. " +
                    $"Valid: {string.Join(", ", ValidRegions)}");

        RuleFor(x => x.Size)
            .NotEmpty()
                .WithErrorCode("Vm.Size.Required")
            .Must(s => ValidSizes.Contains(s))
                .WithErrorCode("Vm.Size.Invalid")
                .WithMessage(x =>
                    $"'{x.Size}' is not a valid size. " +
                    $"Valid: {string.Join(", ", ValidSizes)}");

        RuleFor(x => x.OsType)
            .NotEmpty()
                .WithErrorCode("Vm.OsType.Required")
            .Must(o => o is "Linux" or "Windows")
                .WithErrorCode("Vm.OsType.Invalid")
                .WithMessage("OS type must be 'Linux' or 'Windows'.");

        RuleFor(x => x.ResourceGroup)
            .NotEmpty()
                .WithErrorCode("Vm.ResourceGroup.Required")
            .MaximumLength(90)
                .WithErrorCode("Vm.ResourceGroup.TooLong");
    }
}
