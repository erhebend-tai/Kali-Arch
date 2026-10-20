using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects.Compute;
using FluentAssertions;
using Xunit;

namespace CleanArch.Domain.Tests.ValueObjects;

public sealed class ResourceNameTests
{
    [Theory]
    [InlineData("vm-01")]
    [InlineData("a")]
    [InlineData("my-production-vm-2024")]
    public void Constructor_ValidName_Succeeds(string name) =>
        new ResourceName(name).Value.Should().Be(name);

    [Theory]
    [InlineData("")]
    [InlineData("UPPERCASE")]
    [InlineData("has spaces")]
    [InlineData("-leading-hyphen")]
    [InlineData("trailing-hyphen-")]
    [InlineData("under_score")]
    public void Constructor_InvalidName_Throws(string name)
    {
        var act = () => new ResourceName(name);

        act.Should().Throw<InvalidDomainStateException>();
    }

    [Fact]
    public void Constructor_TooLong_Throws()
    {
        var name = new string('a', 64);

        var act = () => new ResourceName(name);

        act.Should().Throw<InvalidDomainStateException>();
    }
}

public sealed class RegionTests
{
    [Theory]
    [InlineData("us-east")]
    [InlineData("europe-west")]
    [InlineData("asia-east")]
    public void Constructor_SupportedRegion_Succeeds(string region) =>
        new Region(region).Value.Should().Be(region);

    [Theory]
    [InlineData("moon-1")]
    [InlineData("us-central-1")]
    [InlineData("")]
    public void Constructor_UnsupportedRegion_Throws(string region)
    {
        var act = () => new Region(region);

        act.Should().Throw<InvalidDomainStateException>();
    }
}

public sealed class MachineSizeTests
{
    [Theory]
    [InlineData("small")]
    [InlineData("medium")]
    [InlineData("xxlarge")]
    public void Constructor_ValidSize_Succeeds(string size) =>
        new MachineSize(size).Value.Should().Be(size);

    [Fact]
    public void Constructor_InvalidSize_Throws()
    {
        var act = () => new MachineSize("enormous");

        act.Should().Throw<InvalidDomainStateException>();
    }
}
