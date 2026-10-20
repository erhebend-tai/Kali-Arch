using System.Collections.Immutable;
using System.Composition;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace CleanArch.CodeFixes;

/// <summary>
/// Fixes CLEAN0004 — converts a public setter to private.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
[Shared]
public sealed class AddPrivateSetterCodeFix : CodeFixProvider
{
    public override ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create("CLEAN0004");

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override async Task RegisterCodeFixesAsync(
        CodeFixContext context)
    {
        var diagnostic = context.Diagnostics.First();

        context.RegisterCodeFix(
            CodeAction.Create(
                "Make setter private",
                ct => MakeSetterPrivateAsync(context.Document,
                    diagnostic, ct),
                equivalenceKey: "MakeSetterPrivate"),
            diagnostic);

        await Task.CompletedTask;
    }

    private static async Task<Document> MakeSetterPrivateAsync(
        Document document, Diagnostic diagnostic,
        CancellationToken ct)
    {
        var root = await document.GetSyntaxRootAsync(ct)
            .ConfigureAwait(false);
        if (root is null) return document;

        var setter = root.FindNode(diagnostic.Location.SourceSpan)
            as AccessorDeclarationSyntax;
        if (setter is null) return document;

        var privateSetter = setter.WithModifiers(
            SyntaxFactory.TokenList(
                SyntaxFactory.Token(SyntaxKind.PrivateKeyword)));

        return document.WithSyntaxRoot(
            root.ReplaceNode(setter, privateSetter));
    }
}
