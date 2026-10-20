using System.Reactive.Linq;

namespace CleanArch.Application.Reactive.Extensions;

public static class ObservableExtensions
{
    /// <summary>Retry with exponential backoff.</summary>
    public static IObservable<T> RetryWithBackoff<T>(
        this IObservable<T> source,
        int maxRetries = 3,
        TimeSpan? initialDelay = null)
    {
        var delay = initialDelay ?? TimeSpan.FromSeconds(1);

        return Observable.Defer(() =>
        {
            var attempt = 0;
            return source.RetryWhen(errors =>
                errors.SelectMany(error =>
                {
                    attempt++;
                    if (attempt > maxRetries)
                        return Observable.Throw<long>(error);

                    var backoff = TimeSpan.FromMilliseconds(
                        delay.TotalMilliseconds *
                        Math.Pow(2, attempt - 1));

                    return Observable.Timer(backoff);
                }));
        });
    }
}
