using System.ComponentModel;
using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Queries.Compute;
using MediatR;
using ModelContextProtocol.Server;

namespace PlatformMcpServer.Tools.Compute;

[McpServerToolType]
public sealed class ComputeTools(ISender sender)
{
    [McpServerTool(Name = "compute_provision"),
     Description("Provision a VM on the configured cloud provider.")]
    public async Task<string> Provision(
        string name, string resourceGroup, string region,
        string osType, string size,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new ProvisionVmCommand(
                name, resourceGroup, region, osType, size), ct);

        return $"Provisioned {result.Name}: {result.VmId}";
    }

    [McpServerTool(Name = "compute_status"),
     Description("Get the status of every VM.")]
    public async Task<string> Status(CancellationToken ct = default)
    {
        var vms = await sender.Send(
            new GetAllVmsQuery(), ct);

        return string.Join("\n",
            vms.Select(v => $"{v.Name}: {v.Status}"));
    }
}
