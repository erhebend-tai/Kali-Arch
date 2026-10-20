using System.ComponentModel.DataAnnotations;

namespace CleanArch.Application.Options;

public sealed class MessagingOptions
{
    public const string SectionName = "Messaging";

    [Required]
    public string PubSubComponent { get; init; } = "pubsub";

    [Range(1, 10)]
    public int MaxDeliveryAttempts { get; init; } = 3;

    public int OutboxPollIntervalSeconds { get; init; } = 5;
}
