using System.Reflection;
using Xunit;

namespace CleanArch.Linting.Tests;

/// <summary>
/// Dependency-rule enforcement at the assembly level —
/// reflection-based, no external framework required.
/// Swap for ArchUnitNET when the rule set outgrows this.
/// </summary>
public sealed class ArchitectureTests
{
    private static readonly Assembly Domain =
        typeof(CleanArch.Domain.Entities.Compute.VirtualMachine).Assembly;
    private static readonly Assembly Application =
        typeof(CleanArch.Application.Commands.Compute.ProvisionVmCommand).Assembly;

    private static IEnumerable<string> ReferencedAssemblies(Assembly assembly) =>
        assembly.GetReferencedAssemblies().Select(r => r.Name!);

    [Fact]
    public void Domain_does_not_reference_infrastructure_or_entry_points()
    {
        var forbidden = new[]
        {
            "CleanArch.Infrastructure",
            "CleanArch.Infrastructure.Common",
            "CleanArch.API",
            "CleanArch.Functions",
            "Microsoft.EntityFrameworkCore",
            "MediatR"
        };

        var refs = ReferencedAssemblies(Domain).ToList();

        foreach (var name in forbidden)
        {
            Assert.DoesNotContain(refs,
                r => r.StartsWith(name, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Application_does_not_reference_infrastructure()
    {
        var refs = ReferencedAssemblies(Application).ToList();

        foreach (var name in new[]
                 {
                     "CleanArch.Infrastructure",
                     "CleanArch.API"
                 })
        {
            Assert.DoesNotContain(refs,
                r => r.StartsWith(name, StringComparison.Ordinal));
        }
    }

    [Fact]
    public void Domain_types_do_not_use_infrastructure_namespaces()
    {
        var forbiddenPrefixes = new[]
        {
            "Microsoft.EntityFrameworkCore",
            "Azure.",
            "Amazon.",
            "Google.Cloud",
            "Dapper"
        };

        var offenders = Domain.GetTypes()
            .Where(t => t.IsClass)
            .SelectMany(t => t.GetMethods(
                    BindingFlags.Public | BindingFlags.NonPublic |
                    BindingFlags.Instance | BindingFlags.Static)
                .Select(m => (Type: t, Method: m)))
            .Where(x =>
                x.Method.GetParameters()
                    .Select(p => p.ParameterType.Namespace ?? "")
                    .Concat([x.Method.ReturnType.Namespace ?? ""])
                    .Any(ns => forbiddenPrefixes.Any(
                        p => ns.StartsWith(p, StringComparison.Ordinal))))
            .Select(x => x.Type.FullName!)
            .Distinct()
            .ToList();

        Assert.Empty(offenders);
    }

    [Fact]
    public void Interfaces_in_domain_and_application_are_public()
    {
        var nonPublic = Domain.GetTypes()
            .Concat(Application.GetTypes())
            .Where(t => t.IsInterface && !t.IsPublic)
            .Select(t => t.FullName!)
            .ToList();

        Assert.Empty(nonPublic);
    }
}
