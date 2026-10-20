using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.ValueObjects.Compute;

namespace CleanArch.Domain.Specifications.Compute;

public sealed class VmsByRegionSpecification(Region region)
    : BaseSpecification<VirtualMachine>(vm => vm.Region == region);
