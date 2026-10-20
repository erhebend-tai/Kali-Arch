using System.Data.Common;
using Npgsql;
using VirtualMachineService.Application.Interfaces;

namespace VirtualMachineService.Infrastructure.Persistence;

public sealed class NpgsqlConnectionFactory(string connectionString)
    : IDbConnectionFactory
{
    public async Task<DbConnection> CreateConnectionAsync(
        CancellationToken ct = default)
    {
        var conn = new NpgsqlConnection(connectionString);
        await conn.OpenAsync(ct);
        return conn;
    }
}
