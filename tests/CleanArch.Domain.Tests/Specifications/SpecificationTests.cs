using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Specifications.Compute;
using CleanArch.Domain.ValueObjects.Compute;
using FluentAssertions;
using Xunit;

namespace CleanArch.Domain.Tests.Specifications;

public sealed class SpecificationTests
{
    private static VirtualMachine Vm(
        string name, string region, VmStatus status) =>
        VirtualMachine.Reconstitute(
            Guid.NewGuid(),
            new ResourceName(name),
            new Region(region),
            new MachineSize("medium"),
            OsType.Linux,
            status,
            "rg",
            DateTime.UtcNow);

    [Fact]
    public void RunningVms_MatchesOnlyRunning()
    {
        var spec = new RunningVmsSpecification();
        var predicate = spec.Criteria.Compile();

        predicate(Vm("a", "us-east", VmStatus.Running))
            .Should().BeTrue();
        predicate(Vm("b", "us-east", VmStatus.Stopped))
            .Should().BeFalse();
    }

    [Fact]
    public void VmsByRegion_MatchesRegion()
    {
        var spec = new VmsByRegionSpecification(new Region("us-east"));
        var predicate = spec.Criteria.Compile();

        predicate(Vm("a", "us-east", VmStatus.Stopped))
            .Should().BeTrue();
        predicate(Vm("b", "europe-west", VmStatus.Stopped))
            .Should().BeFalse();
    }

    [Fact]
    public void And_ComposesTwoSpecs()
    {
        var spec = new RunningVmsSpecification()
            .And(new VmsByRegionSpecification(new Region("us-east")));
        var predicate = spec.Criteria.Compile();

        predicate(Vm("a", "us-east", VmStatus.Running))
            .Should().BeTrue();
        predicate(Vm("b", "europe-west", VmStatus.Running))
            .Should().BeFalse();
        predicate(Vm("c", "us-east", VmStatus.Stopped))
            .Should().BeFalse();
    }

    [Fact]
    public void Or_MatchesEitherSpec()
    {
        var spec = new VmsByRegionSpecification(new Region("us-east"))
            .Or(new VmsByRegionSpecification(new Region("europe-west")));
        var predicate = spec.Criteria.Compile();

        predicate(Vm("a", "us-east", VmStatus.Stopped)).Should().BeTrue();
        predicate(Vm("b", "europe-west", VmStatus.Stopped))
            .Should().BeTrue();
        predicate(Vm("c", "asia-east", VmStatus.Stopped))
            .Should().BeFalse();
    }

    [Fact]
    public void Not_InvertsSpec()
    {
        var spec = new RunningVmsSpecification().Not();
        var predicate = spec.Criteria.Compile();

        predicate(Vm("a", "us-east", VmStatus.Running))
            .Should().BeFalse();
        predicate(Vm("b", "us-east", VmStatus.Stopped))
            .Should().BeTrue();
    }

    [Fact]
    public void IsSatisfiedBy_CompilesAndEvaluates()
    {
        var spec = new RunningVmsSpecification();

        spec.IsSatisfiedBy(Vm("a", "us-east", VmStatus.Running))
            .Should().BeTrue();
        spec.IsSatisfiedBy(Vm("b", "us-east", VmStatus.Stopped))
            .Should().BeFalse();
    }
}
