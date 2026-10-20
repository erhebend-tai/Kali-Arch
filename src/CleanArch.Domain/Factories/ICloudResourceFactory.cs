namespace CleanArch.Domain.Factories;

/// <summary>
/// Abstract Factory — creates families of related factories.
/// One concrete implementation per provider in Infrastructure.{provider}/Factories.
/// </summary>
public interface ICloudResourceFactory
{
    IVirtualMachineFactory CreateVmFactory();
}
