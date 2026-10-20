using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace VirtualMachineService.Infrastructure.Persistence;

/// <summary>
/// Design-time factory so `dotnet ef migrations add` works without
/// a running host. Override via the VirtualMachineDb env var.
/// </summary>
public sealed class DesignTimeDbContextFactory
    : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("VirtualMachineDb")
            ?? "Host=localhost;Port=5432;Database=platform;" +
               "Username=postgres;Password=postgres";

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(connectionString)
            .Options;

        return new AppDbContext(options);
    }
}
