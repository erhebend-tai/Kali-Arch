using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanArch.Analyzers.Domain;

/// <summary>
/// Flags types inside *.Domain projects that reference
/// infrastructure or cloud-provider namespaces.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DomainLayerAnalyzer : DiagnosticAnalyzer
{
    private static readonly string[] ForbiddenPrefixes =
    [
        "Microsoft.EntityFrameworkCore",
        "Azure.",
        "Amazon.",
        "Google.Cloud",
        "Aliyun.",
        "Oracle.ManagedDataAccess",
        "Cloudflare",
        "Microsoft.Data.SqlClient",
        "Npgsql",
        "Dapper",
        "StackExchange.Redis"
    ];

    public override ImmutableArray<DiagnosticDescriptor>
        SupportedDiagnostics =>
        ImmutableArray.Create(DiagnosticDescriptors.DomainLayerViolation);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(compilationContext =>
        {
            // Only enforce inside projects named *.Domain
            var assemblyName =
                compilationContext.Compilation.AssemblyName ?? "";
            if (!assemblyName.EndsWith(".Domain"))
                return;

            compilationContext.RegisterSyntaxNodeAction(
                AnalyzeUsing, SyntaxKind.UsingDirective);
        });
    }

    private static void AnalyzeUsing(SyntaxNodeAnalysisContext context)
    {
        var usingDirective =
            (Microsoft.CodeAnalysis.CSharp.Syntax.UsingDirectiveSyntax)
                context.Node;

        var name = usingDirective.Name?.ToString();
        if (name is null) return;

        foreach (var forbidden in ForbiddenPrefixes)
        {
            if (name.StartsWith(forbidden) || name == forbidden.TrimEnd('.'))
            {
                var typeName = usingDirective.Parent?
                    .DescendantNodes()
                    .FirstOrDefault()?
                    .ToString() ?? "unknown";

                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.DomainLayerViolation,
                    usingDirective.GetLocation(),
                    typeName,
                    name));
            }
        }
    }
}
