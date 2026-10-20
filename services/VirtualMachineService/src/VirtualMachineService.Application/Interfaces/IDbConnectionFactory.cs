using System.Data.Common;

namespace VirtualMachineService.Application.Interfaces;

/// <summary>
/// Raw-connection abstraction for Dapper/procedure calls —
/// keeps provider types (Npgsql) out of Application.
/// </summary>
public interface IDbConnectionFactory
{
    Task<DbConnection> CreateConnectionAsync(
        CancellationToken ct = default);
}
