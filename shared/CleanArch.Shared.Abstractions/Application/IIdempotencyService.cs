namespace CleanArch.Shared.Abstractions.Application;

/// <summary>
/// Inbox-side deduplication - an idempotency key maps to at most
/// one processed request, and its stored response is replayed.
/// </summary>
public interface IIdempotencyService
{
    Task<bool> IsNewRequestAsync(
        string idempotencyKey, CancellationToken ct = default);

    Task<T?> GetCachedResponseAsync<T>(
        string idempotencyKey, CancellationToken ct = default);

    Task StoreResponseAsync<T>(
        string idempotencyKey,
        T response,
        TimeSpan? expiry = null,
        CancellationToken ct = default);
}
