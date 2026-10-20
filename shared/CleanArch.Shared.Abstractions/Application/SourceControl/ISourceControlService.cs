using CleanArch.Shared.Abstractions.DTOs.SourceControl;

namespace CleanArch.Shared.Abstractions.Application.SourceControl;

/// <summary>
/// Source control platform - GitHub / GitLab / Azure DevOps.
/// </summary>
public interface ISourceControlService
{
    Task<string> CreateIssueAsync(
        string repository,
        string title,
        string body,
        CancellationToken ct = default);

    Task<IReadOnlyList<RepositoryInfo>> ListRepositoriesAsync(
        CancellationToken ct = default);

    Task<PullRequestInfo> CreatePullRequestAsync(
        CreatePrRequest request, CancellationToken ct = default);

    Task<IReadOnlyList<WorkflowRunInfo>> GetWorkflowRunsAsync(
        string repository, CancellationToken ct = default);

    Task TriggerWorkflowAsync(
        string repository, string workflow,
        IReadOnlyDictionary<string, string>? inputs = null,
        CancellationToken ct = default);
}
