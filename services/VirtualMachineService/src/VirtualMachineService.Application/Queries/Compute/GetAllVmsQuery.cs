using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Common.Models;
using VirtualMachineService.Application.DTOs.Compute;
using VirtualMachineService.Application.Interfaces;

namespace VirtualMachineService.Application.Queries.Compute;

public sealed record GetAllVmsQuery(
    string TenantId,
    string? Status = null,
    string? Region = null,
    string? Search = null,
    int Page = 1,
    int PageSize = 20) : IQuery<PagedResult<VmSummaryDto>>;

public sealed class GetAllVmsQueryHandler(
    IVirtualMachineRepository repository)
    : IQueryHandler<GetAllVmsQuery, PagedResult<VmSummaryDto>>
{
    public Task<PagedResult<VmSummaryDto>> Handle(
        GetAllVmsQuery request, CancellationToken ct) =>
        repository.GetByTenantAsync(
            request.TenantId,
            new VmFilter(
                request.Status,
                request.Region,
                request.Search,
                request.Page,
                request.PageSize),
            ct);
}
