using CleanArch.Domain.Entities.Compute;

namespace CleanArch.Domain.Specifications.Compute;

public sealed class VmsByOwnerSpecification(string createdBy)
    : BaseSpecification<VirtualMachine>(vm => vm.CreatedBy == createdBy);
