using CleanArch.Application.DTOs.Compute;
using CleanArch.Application.Interfaces.Reports;

namespace CleanArch.Application.Reports.Templates;

/// <summary>
/// Builds a ReportRequest from VM data — the template knows
/// the shape; the generator knows the format.
/// </summary>
public static class VmInventoryReport
{
    public static ReportRequest Build(
        IEnumerable<VmDetailsDto> vms) =>
        new(
            Title: "VM Inventory Report",
            Columns: ["Name", "Region", "Size", "OS", "Status", "Created"],
            Rows: vms.Select(vm => (IReadOnlyList<string>)
                [
                    vm.Name,
                    vm.Region,
                    vm.Size,
                    vm.OsType,
                    vm.Status,
                    vm.CreatedAt.ToString("yyyy-MM-dd")
                ]).ToList(),
            Summary: $"Total VMs: {vms.Count()}");
}
