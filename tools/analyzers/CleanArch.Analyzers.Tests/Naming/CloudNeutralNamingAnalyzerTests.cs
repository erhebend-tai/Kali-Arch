using CleanArch.Analyzers.Naming;
using CleanArch.Analyzers.Tests.Helpers;
using Xunit;

namespace CleanArch.Analyzers.Tests.Naming;

public sealed class CloudNeutralNamingAnalyzerTests
{
    [Fact]
    public async Task S3_in_interface_name_is_flagged()
    {
        const string source = """
            public interface IS3StorageService { }
            """;

        var diagnostics = await AnalyzerTestHelper.RunAnalyzerAsync(
            new CloudNeutralNamingAnalyzer(), source);

        Assert.Contains(diagnostics, d => d.Id == "CLEAN0002");
    }

    [Fact]
    public async Task Neutral_interface_name_is_clean()
    {
        const string source = """
            public interface IObjectStorageService { }
            """;

        var diagnostics = await AnalyzerTestHelper.RunAnalyzerAsync(
            new CloudNeutralNamingAnalyzer(), source);

        Assert.DoesNotContain(diagnostics, d => d.Id == "CLEAN0002");
    }
}
