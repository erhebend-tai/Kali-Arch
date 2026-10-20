using CleanArch.Domain.Entities.Compute;
using CleanArch.Domain.Enums.Compute;
using CleanArch.Domain.Factories;
using CleanArch.Domain.Factories.Compute;
using CleanArch.Domain.Specifications.Compute;
using CleanArch.Domain.ValueObjects.Compute;
using CleanArch.Infrastructure.Common.Persistence;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Testcontainers.MsSql;
using Xunit;

namespace CleanArch.IntegrationTests.Persistence;

/// <summary>
/// EF Core against a real SQL Server in Docker —
/// catches provider-specific issues an InMemory store hides
/// (owned types, query translation, constraints).
/// </summary>
public sealed class VmRepositoryTests
    : IAsyncLifetime
{
    private readonly MsSqlContainer _container =
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-latest")
            .Build();

    private AppDbContext _context = null!;
    private readonly IVirtualMachineFactory _factory =
        new VirtualMachineFactory();

    public async Task InitializeAsync()
    {
        await _container.StartAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(_container.GetConnectionString())
            .Options;

        _context = new AppDbContext(options);
        await _context.Database.EnsureCreatedAsync();
    }

    public async Task DisposeAsync()
    {
        await _context.DisposeAsync();
        await _container.DisposeAsync();
    }

    [Fact]
    public async Task AddAndRetrieve_RoundTripsEntity()
    {
        var vm = _factory.Create(
            "web-01", "us-east", "medium",
            "Linux", "rg-1", "test-user");

        var repo = new VirtualMachineRepository(_context);
        await repo.AddAsync(vm, CancellationToken.None);
        await _context.SaveChangesAsync();

        var loaded = await repo.GetByIdAsync(vm.Id, CancellationToken.None);

        loaded.Should().NotBeNull();
        loaded!.Name.Value.Should().Be("web-01");
        loaded.Region.Value.Should().Be("us-east");
        loaded.Size.Value.Should().Be("medium");
    }

    [Fact]
    public async Task FindAsync_RunningVmsSpec_FiltersCorrectly()
    {
        var repo = new VirtualMachineRepository(_context);

        var running = _factory.Create(
            "running-vm", "us-east", "small",
            "Linux", "rg-1", "test-user");
        running.Start();

        var stopped = _factory.Create(
            "stopped-vm", "us-east", "small",
            "Linux", "rg-1", "test-user");

        await repo.AddAsync(running, CancellationToken.None);
        await repo.AddAsync(stopped, CancellationToken.None);
        await _context.SaveChangesAsync();

        var found = await repo.FindAsync(
            new RunningVmsSpecification(), CancellationToken.None);

        found.Should().ContainSingle()
            .Which.Name.Value.Should().Be("running-vm");
    }
}
