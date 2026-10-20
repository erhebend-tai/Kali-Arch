using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;

namespace CleanArch.Domain.Specifications.Compute;

public sealed class StaleVmsSpecification(TimeSpan idleThreshold)
    : BaseSpecification<VirtualMachine>(
        vm => vm.Status == VmStatus.Stopped
           && vm.UpdatedAt.HasValue
           && DateTime.UtcNow - vm.UpdatedAt.Value > idleThreshold);
