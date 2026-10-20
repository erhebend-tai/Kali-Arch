using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Validators.Compute;
using FluentAssertions;
using Xunit;

namespace CleanArch.Application.Tests.Validators;

public sealed class ProvisionVmCommandValidatorTests
{
    private readonly ProvisionVmCommandValidator _validator = new();

    private static ProvisionVmCommand Valid() =>
        new("web-01", "prod-rg", "us-east", "Linux", "medium");

    [Fact]
    public void ValidCommand_Passes()
    {
        var result = _validator.Validate(Valid());

        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("UPPERCASE")]
    [InlineData("has space")]
    public void InvalidName_Fails(string name)
    {
        var cmd = new ProvisionVmCommand(
            name, "rg", "us-east", "Linux", "medium");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e =>
            e.PropertyName == nameof(ProvisionVmCommand.Name));
    }

    [Fact]
    public void InvalidRegion_Fails()
    {
        var cmd = new ProvisionVmCommand(
            "web-01", "rg", "moon-1", "Linux", "medium");

        var result = _validator.Validate(cmd);

        result.Errors.Should().Contain(e =>
            e.ErrorCode == "Vm.Region.Invalid");
    }

    [Fact]
    public void InvalidSize_Fails()
    {
        var cmd = new ProvisionVmCommand(
            "web-01", "rg", "us-east", "Linux", "huge");

        var result = _validator.Validate(cmd);

        result.Errors.Should().Contain(e =>
            e.ErrorCode == "Vm.Size.Invalid");
    }

    [Theory]
    [InlineData("FreeBSD")]
    [InlineData("")]
    public void InvalidOsType_Fails(string osType)
    {
        var cmd = new ProvisionVmCommand(
            "web-01", "rg", "us-east", osType, "medium");

        var result = _validator.Validate(cmd);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void ErrorCodes_Stable()
    {
        var cmd = new ProvisionVmCommand(
            "", "", "", "", "");

        var result = _validator.Validate(cmd);

        var codes = result.Errors.Select(e => e.ErrorCode).ToList();
        codes.Should().Contain("Vm.Name.Required");
        codes.Should().Contain("Vm.Region.Required");
        codes.Should().Contain("Vm.Size.Required");
    }
}
