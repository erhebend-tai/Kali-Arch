using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Events.Compute;
using CleanArch.Domain.Exceptions;
using CleanArch.Domain.ValueObjects.Compute;
using FluentAssertions;
using Xunit;

namespace CleanArch.Domain.Tests.Entities;

public sealed class VirtualMachineTests
{
    private static VirtualMachine CreateVm(
        string name = "test-vm",
        string region = "us-east",
        string size = "medium") =>
        VirtualMachine.Create(
            new ResourceName(name),
            new Region(region),
            new MachineSize(size),
            OsType.Linux,
            "test-rg",
            "test-user");

    [Fact]
    public void Create_ValidInput_RaisesProvisionedEvent()
    {
        var vm = CreateVm();

        vm.Status.Should().Be(VmStatus.Stopped);
        vm.Name.Value.Should().Be("test-vm");
        vm.DomainEvents.Should().ContainSingle()
            .Which.Should().BeOfType<VmProvisionedEvent>();
    }

    [Fact]
    public void Start_WhenStopped_TransitionsToRunning()
    {
        var vm = CreateVm();

        vm.Start();

        vm.Status.Should().Be(VmStatus.Running);
        vm.DomainEvents.Should().Contain(e => e is VmStartedEvent);
    }

    [Fact]
    public void Start_WhenAlreadyRunning_ThrowsInvalidDomainState()
    {
        var vm = CreateVm();
        vm.Start();

        var act = () => vm.Start();

        act.Should().Throw<InvalidDomainStateException>()
            .WithMessage("*already running*");
    }

    [Fact]
    public void Stop_WhenRunning_TransitionsToStopped()
    {
        var vm = CreateVm();
        vm.Start();

        vm.Stop();

        vm.Status.Should().Be(VmStatus.Stopped);
    }

    [Fact]
    public void Stop_WhenAlreadyStopped_ThrowsInvalidDomainState()
    {
        var vm = CreateVm();

        var act = () => vm.Stop();

        act.Should().Throw<InvalidDomainStateException>()
            .WithMessage("*already stopped*");
    }

    [Fact]
    public void Resize_WhenRunning_ThrowsInvalidDomainState()
    {
        var vm = CreateVm();
        vm.Start();

        var act = () => vm.Resize(new MachineSize("large"));

        act.Should().Throw<InvalidDomainStateException>()
            .WithMessage("*stopped*");
    }

    [Fact]
    public void Resize_WhenStopped_UpdatesSize()
    {
        var vm = CreateVm();

        vm.Resize(new MachineSize("xlarge"));

        vm.Size.Value.Should().Be("xlarge");
    }

    [Fact]
    public void Reconstitute_DoesNotRaiseDomainEvents()
    {
        var vm = VirtualMachine.Reconstitute(
            Guid.NewGuid(),
            new ResourceName("old-vm"),
            new Region("us-east"),
            new MachineSize("small"),
            OsType.Windows,
            VmStatus.Running,
            "rg",
            DateTime.UtcNow.AddDays(-1));

        vm.DomainEvents.Should().BeEmpty();
        vm.Status.Should().Be(VmStatus.Running);
    }
}
