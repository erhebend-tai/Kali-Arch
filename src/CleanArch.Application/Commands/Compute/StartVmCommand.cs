using CleanArch.Application.Common;

namespace CleanArch.Application.Commands.Compute;

public sealed record StartVmCommand(Guid VmId) : ICommand;
