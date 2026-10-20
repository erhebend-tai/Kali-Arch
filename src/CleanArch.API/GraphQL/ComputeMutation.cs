using CleanArch.Application.Commands.Compute;
using MediatR;

namespace CleanArch.API.GraphQL;

public sealed class ComputeMutation
{
    public async Task<ProvisionVmResult> ProvisionVm(
        ProvisionVmInput input,
        [Service] ISender sender,
        CancellationToken ct) =>
        await sender.Send(
            new ProvisionVmCommand(
                input.Name, input.ResourceGroup,
                input.Region, input.OsType, input.Size),
            ct);

    public async Task<bool> StartVm(
        Guid id,
        [Service] ISender sender,
        CancellationToken ct)
    {
        await sender.Send(new StartVmCommand(id), ct);
        return true;
    }
}

public sealed record ProvisionVmInput(
    string Name,
    string ResourceGroup,
    string Region,
    string OsType,
    string Size);
