namespace CleanArch.Shared.Abstractions.DTOs.Payment;

public sealed record PaymentResult(
    string TransactionId,
    PaymentStatus Status,
    decimal Amount,
    string Currency);
