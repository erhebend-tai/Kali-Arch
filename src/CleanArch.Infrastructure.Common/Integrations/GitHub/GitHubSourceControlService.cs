using CleanArch.Application.Interfaces.SourceControl;
using Octokit;

namespace CleanArch.Infrastructure.Common.Integrations.GitHub;

/// <summary>Octokit-backed ISourceControlService.</summary>
public sealed class GitHubSourceControlService(
    GitHubClient client) : ISourceControlService
{
    public async Task<string> CreateIssueAsync(
        string repository, string title, string body,
        CancellationToken ct = default)
    {
        var parts = repository.Split('/');
        var issue = await client.Issue.Create(
            parts[0], parts[1],
            new NewIssue(title) { Body = body });

        return issue.HtmlUrl;
    }

    public async Task<IReadOnlyList<RepositoryInfo>>
        ListRepositoriesAsync(CancellationToken ct = default)
    {
        var repos = await client.Repository
            .GetAllForCurrent();

        return repos.Select(r => new RepositoryInfo(
            r.Name, r.FullName, r.Description,
            r.DefaultBranch)).ToList();
    }
}
