using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;

namespace CleanArch.Domain.Specifications.Compute;

public sealed class RunningVmsSpecification()
    : BaseSpecification<VirtualMachine>(
        vm => vm.Status == VmStatus.Running);
