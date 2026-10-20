namespace VirtualMachineService.Domain.Exceptions;

public sealed class InvalidDomainStateException(string message)
    : DomainException(message);
