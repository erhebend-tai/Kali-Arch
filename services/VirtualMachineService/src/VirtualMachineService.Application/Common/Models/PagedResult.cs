namespace VirtualMachineService.Application.Common.Models;

public sealed record PagedResult<T>(
    IEnumerable<T> Items,
    long TotalCount,
    int Page,
    int PageSize);
