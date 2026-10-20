namespace CleanArch.Domain.Exceptions;

public sealed class QuotaExceededException(string region)
    : DomainException($"Quota exceeded in region '{region}'.")
{
    public string Region { get; } = region;
}
