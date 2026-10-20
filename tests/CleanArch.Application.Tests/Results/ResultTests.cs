using CleanArch.Shared.Results;
using FluentAssertions;
using Xunit;

namespace CleanArch.Application.Tests.Results;

public sealed class ResultTests
{
    [Fact]
    public void Success_Result_HasValueAndNoError()
    {
        var result = Result<int>.Success(42);

        result.IsSuccess.Should().BeTrue();
        result.IsFailure.Should().BeFalse();
        result.Value.Should().Be(42);
        result.Error.Should().Be(Error.None);
    }

    [Fact]
    public void Failure_Result_CarriesError()
    {
        var error = Error.NotFound("Vm.NotFound", "VM not found.");
        var result = Result.Failure<int>(error);

        result.IsSuccess.Should().BeFalse();
        result.Error.Code.Should().Be("Vm.NotFound");
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public void Value_OnFailure_Throws()
    {
        var result = Result.Failure<int>(
            Error.Failure("x", "boom"));

        var act = () => result.Value;

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void ErrorFactories_SetCorrectType()
    {
        Error.Validation("c", "d").Type.Should().Be(ErrorType.Validation);
        Error.NotFound("c", "d").Type.Should().Be(ErrorType.NotFound);
        Error.Conflict("c", "d").Type.Should().Be(ErrorType.Conflict);
        Error.Unauthorized("c", "d").Type
            .Should().Be(ErrorType.Unauthorized);
        Error.Forbidden("c", "d").Type.Should().Be(ErrorType.Forbidden);
        Error.Domain("c", "d").Type.Should().Be(ErrorType.Domain);
    }
}
