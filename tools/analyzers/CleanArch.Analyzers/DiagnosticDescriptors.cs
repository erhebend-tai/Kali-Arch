using Microsoft.CodeAnalysis;

namespace CleanArch.Analyzers;

internal static class DiagnosticDescriptors
{
    public static readonly DiagnosticDescriptor DomainLayerViolation = new(
        id: "CLEAN0001",
        title: "Domain layer references infrastructure",
        messageFormat: "Type '{0}' in the Domain layer references '{1}'. " +
                       "Domain must not depend on infrastructure or cloud SDKs.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor CloudSpecificNaming = new(
        id: "CLEAN0002",
        title: "Cloud-specific naming in abstractions",
        messageFormat: "Name '{0}' contains provider-specific term '{1}'. " +
                       "Use a cloud-neutral name instead.",
        category: "Naming",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor HardcodedSecret = new(
        id: "CLEAN0003",
        title: "Possible hardcoded secret",
        messageFormat: "Possible hardcoded secret in literal for '{0}'. " +
                       "Use configuration or a secret store.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor EntityPublicSetter = new(
        id: "CLEAN0004",
        title: "Entity property must have private setter",
        messageFormat: "Property '{0}' on entity type '{1}' has a public setter. " +
                       "Use private setters + behavior methods.",
        category: "Domain",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
}
