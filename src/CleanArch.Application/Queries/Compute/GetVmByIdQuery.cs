using CleanArch.Application.Common;

namespace CleanArch.Application.Queries.Compute;

public sealed record GetVmByIdQuery(Guid VmId) : IQuery<VmDetails?>;

public sealed record VmDetails(
    Guid Id,
    string Name,
    string Region,
    string Size,
    string OsType,
    string Status,
    string ResourceGroup,
    DateTime CreatedAt);
