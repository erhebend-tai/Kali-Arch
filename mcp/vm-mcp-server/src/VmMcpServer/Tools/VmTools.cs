using System.ComponentModel;
using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Queries.Compute;
using MediatR;
using ModelContextProtocol.Server;

namespace VmMcpServer.Tools;

/// <summary>
/// MCP tools — thin adapters over the same ISender pipeline
/// every other entry point uses.
/// </summary>
[McpServerToolType]
public sealed class VmTools(ISender sender)
{
    [McpServerTool(Name = "provision_vm"),
     Description("Provision a new virtual machine.")]
    public async Task<string> ProvisionVm(
        [Description("VM name (lowercase alphanumeric + hyphens)")]
        string name,
        [Description("Resource group")] string resourceGroup,
        [Description("Cloud region, e.g. us-east")] string region,
        [Description("OS: Linux or Windows")] string osType,
        [Description("Size tier: small|medium|large|xlarge|xxlarge")]
        string size,
        CancellationToken ct = default)
    {
        var result = await sender.Send(
            new ProvisionVmCommand(
                name, resourceGroup, region, osType, size), ct);

        return $"VM provisioned: {result.VmId} ({result.Status})";
    }

    [McpServerTool(Name = "start_vm"),
     Description("Start a stopped virtual machine.")]
    public async Task<string> StartVm(
        [Description("VM ID (GUID)")] string vmId,
        CancellationToken ct = default)
    {
        await sender.Send(new StartVmCommand(Guid.Parse(vmId)), ct);
        return $"VM {vmId} started.";
    }

    [McpServerTool(Name = "list_vms"),
     Description("List virtual machines, optionally filtered.")]
    public async Task<string> ListVms(
        [Description("Optional region filter")] string? region = null,
        [Description("Only running VMs")] bool runningOnly = false,
        CancellationToken ct = default)
    {
        var vms = await sender.Send(
            new GetAllVmsQuery(region, runningOnly), ct);

        return string.Join("\n",
            vms.Select(v =>
                $"{v.Id}  {v.Name}  {v.Region}  {v.Size}  {v.Status}"));
    }
}
