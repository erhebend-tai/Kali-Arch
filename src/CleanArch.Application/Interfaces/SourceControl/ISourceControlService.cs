namespace CleanArch.Application.Interfaces.SourceControl;

public interface ISourceControlService
{
    Task<string> CreateIssueAsync(
        string repository,
        string title,
        string body,
        CancellationToken ct = default);

    Task<IReadOnlyList<RepositoryInfo>> ListRepositoriesAsync(
        CancellationToken ct = default);
}

public sealed record RepositoryInfo(
    string Name,
    string FullName,
    string? Description,
    string DefaultBranch);
