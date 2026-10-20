using CleanArch.Application.Queries.Compute;
using MediatR;

namespace CleanArch.API.GraphQL;

/// <summary>
/// GraphQL read side — same ISender pipeline as REST/gRPC.
/// </summary>
public sealed class ComputeQuery
{
    public async Task<VmDetails?> GetVm(
        Guid id,
        [Service] ISender sender,
        CancellationToken ct) =>
        await sender.Send(new GetVmByIdQuery(id), ct);

    public async Task<IReadOnlyList<VmDetails>> GetVms(
        [Service] ISender sender,
        string? region = null,
        bool runningOnly = false,
        CancellationToken ct = default) =>
        await sender.Send(new GetAllVmsQuery(region, runningOnly), ct);
}
