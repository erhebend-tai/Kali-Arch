namespace CleanArch.Application.Interfaces.Payment;

public interface IPaymentService
{
    Task<PaymentResult> ChargeAsync(
        PaymentRequest request, CancellationToken ct = default);
}

public sealed record PaymentRequest(
    decimal Amount,
    string Currency,
    string PaymentMethodId,
    string OrderId,
    string CustomerId,
    string? ReturnUrl = null);

public sealed record PaymentResult(
    string TransactionId,
    PaymentStatus Status,
    decimal Amount,
    string Currency);

public enum PaymentStatus
{
    Succeeded,
    Processing,
    RequiresAction,
    Cancelled,
    Failed
}
