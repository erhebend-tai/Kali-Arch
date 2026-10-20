using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CleanArch.Analyzers.Architecture;

/// <summary>
/// Flags public setters on types deriving from Entity /
/// AuditableEntity — entities change state through methods.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EntityPrivateSetterAnalyzer : DiagnosticAnalyzer
{
    public override ImmutableArray<DiagnosticDescriptor>
        SupportedDiagnostics =>
        ImmutableArray.Create(
            DiagnosticDescriptors.EntityPublicSetter);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(
            GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(
            AnalyzeProperty, SyntaxKind.PropertyDeclaration);
    }

    private static void AnalyzeProperty(SyntaxNodeAnalysisContext context)
    {
        var property = (PropertyDeclarationSyntax)context.Node;

        var setter = property.AccessorList?.Accessors
            .FirstOrDefault(a =>
                a.IsKind(SyntaxKind.SetAccessorDeclaration));
        if (setter is null) return;

        // Public setter = no accessibility modifiers on the setter
        if (setter.Modifiers.Any()) return;

        // Only inside entity types — check containing class base list
        var containingType = property.Parent as ClassDeclarationSyntax;
        var isEntity = containingType?.BaseList?.Types
            .Any(t => t.Type.ToString().Contains("Entity")) == true;
        if (!isEntity) return;

        context.ReportDiagnostic(Diagnostic.Create(
            DiagnosticDescriptors.EntityPublicSetter,
            setter.GetLocation(),
            property.Identifier.Text,
            containingType!.Identifier.Text));
    }
}
