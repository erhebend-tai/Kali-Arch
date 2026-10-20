namespace CleanArch.Shared.Abstractions.DTOs.Payment;

public enum PaymentStatus
{
    Succeeded,
    Processing,
    RequiresAction,
    Cancelled,
    Failed
}
