using System.Runtime.CompilerServices;

namespace CleanArch.Shared.Extensions;

public static class EnumerableExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(
        this IEnumerable<T> source, int size)
    {
        var batch = new List<T>(size);
        foreach (var item in source)
        {
            batch.Add(item);
            if (batch.Count != size) continue;
            yield return batch;
            batch = new List<T>(size);
        }
        if (batch.Count > 0) yield return batch;
    }

    public static async IAsyncEnumerable<T> WhereAsync<T>(
        this IEnumerable<T> source,
        Func<T, CancellationToken, Task<bool>> predicate,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        foreach (var item in source)
        {
            if (await predicate(item, ct))
                yield return item;
        }
    }

    public static IEnumerable<T> DistinctBy<T, TKey>(
        this IEnumerable<T> source,
        Func<T, TKey> keySelector)
    {
        var seen = new HashSet<TKey>();
        foreach (var item in source)
            if (seen.Add(keySelector(item)))
                yield return item;
    }

    public static (IEnumerable<T> Passed, IEnumerable<T> Failed)
        Partition<T>(
            this IEnumerable<T> source, Func<T, bool> predicate)
    {
        var passed = new List<T>();
        var failed = new List<T>();

        foreach (var item in source)
            (predicate(item) ? passed : failed).Add(item);

        return (passed, failed);
    }
}
