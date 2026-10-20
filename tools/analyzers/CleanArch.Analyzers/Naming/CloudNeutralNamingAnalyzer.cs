using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanArch.Analyzers.Naming;

/// <summary>
/// Flags provider-specific terms in interface names —
/// abstractions must stay cloud-neutral.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CloudNeutralNamingAnalyzer : DiagnosticAnalyzer
{
    private static readonly (string Term, string Suggestion)[] Terms =
    [
        ("S3", "ObjectStorage"),
        ("Blob", "ObjectStorage"),
        ("ServiceBus", "MessageBus"),
        ("Sqs", "Queue"),
        ("PubSub", "MessageBus"),
        ("Cosmos", "DocumentStore"),
        ("Dynamo", "DocumentStore"),
        ("KeyVault", "SecretStore")
    ];

    public override ImmutableArray<DiagnosticDescriptor>
        SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.CloudSpecificNaming);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeInterface, SyntaxKind.InterfaceDeclaration);
    }

    private static void AnalyzeInterface(SyntaxNodeAnalysisContext context)
    {
        var declaration = (InterfaceDeclarationSyntax)context.Node;
        var name = declaration.Identifier.Text;

        foreach (var (term, _) in Terms)
        {
            if (name.IndexOf(term, System.StringComparison.Ordinal) >= 0)
            {
                context.ReportDiagnostic(Diagnostic.Create(
                    DiagnosticDescriptors.CloudSpecificNaming,
                    declaration.Identifier.GetLocation(),
                    name,
                    term));
            }
        }
    }
}
