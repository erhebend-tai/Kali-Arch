using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanArch.Analyzers.Security;

/// <summary>
/// Flags string literals that look like embedded secrets —
/// key=value connection strings, password=, api_key=, etc.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NoHardcodedSecretsAnalyzer : DiagnosticAnalyzer
{
    private static readonly Regex SecretPattern = new(
        @"(password|pwd|api[_-]?key|secret|token|accountkey|connectionstring)\s*[=:]",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override ImmutableArray<DiagnosticDescriptor>
        SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.HardcodedSecret);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeLiteral, SyntaxKind.StringLiteralExpression);
    }

    private static void AnalyzeLiteral(SyntaxNodeAnalysisContext context)
    {
        var literal = (LiteralExpressionSyntax)context.Node;
        var text = literal.Token.ValueText;

        if (text.Length < 16) return; // too short to be a secret

        if (SecretPattern.IsMatch(text))
        {
            context.ReportDiagnostic(Diagnostic.Create(
                DiagnosticDescriptors.HardcodedSecret,
                literal.GetLocation(),
                text.Length > 24 ? text.Substring(0, 24) + "..." : text));
        }
    }
}
