namespace CleanArch.Shared.Abstractions.DTOs.Payment;

public sealed record PaymentRequest(
    decimal Amount,
    string Currency,
    string PaymentMethodId,
    string OrderId,
    string CustomerId,
    string? ReturnUrl = null);
