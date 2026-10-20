namespace CleanArch.Shared.Results;

public sealed class Result<T> : Result
{
    private readonly T _value;

    internal Result(T value, bool isSuccess, Error error)
        : base(isSuccess, error)
    {
        _value = value;
    }

    public T Value => IsSuccess
        ? _value
        : throw new InvalidOperationException(
            $"Cannot access Value of a failed result. Error: {Error.Code}");

    public static implicit operator Result<T>(T value) => Success(value);
    public static implicit operator Result<T>(Error error) =>
        Failure<T>(error);

    public Result<TNext> Map<TNext>(Func<T, TNext> mapper) =>
        IsSuccess
            ? Success(mapper(Value))
            : Failure<TNext>(Error);

    public Result<TNext> Bind<TNext>(Func<T, Result<TNext>> binder) =>
        IsSuccess ? binder(Value) : Failure<TNext>(Error);

    public async Task<Result<TNext>> BindAsync<TNext>(
        Func<T, Task<Result<TNext>>> binder) =>
        IsSuccess ? await binder(Value) : Failure<TNext>(Error);

    public T GetValueOrDefault(T defaultValue = default!) =>
        IsSuccess ? Value : defaultValue;

    public void Switch(Action<T> onSuccess, Action<Error> onFailure)
    {
        if (IsSuccess) onSuccess(Value);
        else onFailure(Error);
    }

    public async Task SwitchAsync(
        Func<T, Task> onSuccess,
        Func<Error, Task> onFailure)
    {
        if (IsSuccess) await onSuccess(Value);
        else await onFailure(Error);
    }
}
