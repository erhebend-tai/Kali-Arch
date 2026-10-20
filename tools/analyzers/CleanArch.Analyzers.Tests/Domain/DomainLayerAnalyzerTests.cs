using CleanArch.Analyzers.Domain;
using CleanArch.Analyzers.Tests.Helpers;
using Xunit;

namespace CleanArch.Analyzers.Tests.Domain;

public sealed class DomainLayerAnalyzerTests
{
    [Fact]
    public async Task Domain_type_referencing_ef_core_is_flagged()
    {
        const string source = """
            using Microsoft.EntityFrameworkCore;

            namespace CleanArch.Domain.Entities;

            public class MyEntity { }
            """;

        var diagnostics = await AnalyzerTestHelper.RunAnalyzerAsync(
            new DomainLayerAnalyzer(), source, "CleanArch.Domain");

        Assert.Contains(diagnostics,
            d => d.Id == "CLEAN0001");
    }

    [Fact]
    public async Task Domain_type_with_clean_usings_is_not_flagged()
    {
        const string source = """
            using System;
            using System.Linq;

            namespace CleanArch.Domain.Entities;

            public class MyEntity { }
            """;

        var diagnostics = await AnalyzerTestHelper.RunAnalyzerAsync(
            new DomainLayerAnalyzer(), source, "CleanArch.Domain");

        Assert.DoesNotContain(diagnostics,
            d => d.Id == "CLEAN0001");
    }

    [Fact]
    public async Task Non_domain_assembly_is_not_checked()
    {
        const string source = """
            using Microsoft.EntityFrameworkCore;

            namespace CleanArch.Infrastructure.Common;
            """;

        var diagnostics = await AnalyzerTestHelper.RunAnalyzerAsync(
            new DomainLayerAnalyzer(), source,
            "CleanArch.Infrastructure.Common");

        Assert.Empty(diagnostics);
    }
}
