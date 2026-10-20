using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace CleanArch.Analyzers.Tests.Helpers;

/// <summary>
/// Compiles source text in-memory and runs a single analyzer —
/// the lightest possible analyzer test loop.
/// </summary>
public static class AnalyzerTestHelper
{
    public static async Task<ImmutableArray<Diagnostic>> RunAnalyzerAsync(
        DiagnosticAnalyzer analyzer,
        string source,
        string assemblyName = "Test.Assembly")
    {
        var syntaxTree = CSharpSyntaxTree.ParseText(source);

        var references = new[]
        {
            MetadataReference.CreateFromFile(
                typeof(object).Assembly.Location),
            MetadataReference.CreateFromFile(
                typeof(Enumerable).Assembly.Location)
        };

        var compilation = CSharpCompilation.Create(
            assemblyName,
            [syntaxTree],
            references,
            new CSharpCompilationOptions(
                OutputKind.DynamicallyLinkedLibrary));

        var withAnalyzers = compilation.WithAnalyzers(
            ImmutableArray.Create(analyzer));

        return await withAnalyzers.GetAnalyzerDiagnosticsAsync();
    }
}
