using System.ComponentModel;
using ModelContextProtocol.Server;

namespace DocsMcpServer.Resources;

/// <summary>
/// MCP resources — exposes the repo's architecture docs
/// to agents without them needing file access.
/// </summary>
[McpServerResourceType]
public sealed class DocsResources
{
    [McpServerResource(
        UriTemplate = "docs://prompts/{name}",
        Name = "prompt_template",
        MimeType = "text/markdown"),
     Description("Implementation prompt templates from docs/prompts/.")]
    public static string PromptTemplate(string name)
    {
        var path = Path.Combine("docs", "prompts", $"{name}.md");
        return File.Exists(path)
            ? File.ReadAllText(path)
            : $"Prompt '{name}' not found.";
    }

    [McpServerResource(
        UriTemplate = "docs://asyncapi",
        Name = "event_contracts",
        MimeType = "application/yaml"),
     Description("AsyncAPI event contract catalog.")]
    public static string EventContracts()
    {
        var path = Path.Combine("docs", "asyncapi", "asyncapi.yaml");
        return File.Exists(path)
            ? File.ReadAllText(path)
            : "AsyncAPI spec not found.";
    }
}
