using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CleanArch.CodeFixes;

/// <summary>
/// Fixes CLEAN0003 — replaces a hardcoded secret literal with a
/// configuration lookup, e.g. _configuration["SecretKey"].
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class ReplaceHardcodedSecretCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("CLEAN0003");

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(
        CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        context.RegisterCodeFix(
            CodeAction.Create(
                "Move to configuration",
                ct => MoveToConfigurationAsync(context.Document,
                    diagnostic, ct),
                equivalenceKey: "MoveToConfiguration"),
            diagnostic);

        await Task.CompletedTask;
    }

    private static async Task<Document> MoveToConfigurationAsync(
        Document document, Diagnostic diagnostic,
        CancellationToken ct)
    {
        var root = await document.GetSyntaxRootAsync(ct)
            .ConfigureAwait(false);
        if (root is null) return document;

        var literal = root.FindNode(diagnostic.Location.SourceSpan)
            as LiteralExpressionSyntax;
        if (literal is null) return document;

        // "DefaultEndpointsProtocol=https;AccountKey=abc..." → "Secrets:ConnectionString"
        var keyName = GuessKeyName(literal.Token.ValueText);

        var replacement = SyntaxFactory.ParseExpression(
            $"_configuration[\"{keyName}\"]")
            .WithTriviaFrom(literal);

        return document.WithSyntaxRoot(
            root.ReplaceNode(literal, replacement));
    }

    private static string GuessKeyName(string literal)
    {
        if (literal.IndexOf("AccountKey",
                System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "Secrets:StorageConnection";
        if (literal.IndexOf("password",
                System.StringComparison.OrdinalIgnoreCase) >= 0)
            return "Secrets:ConnectionString";
        return "Secrets:ApiKey";
    }
}
