namespace CleanArch.Shared.Abstractions.Domain;

// GoF Command pattern - encapsulates a domain operation as an object.
// NOTE: different from the MediatR ICommand markers.
public interface IDomainCommand
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken ct = default);
}
