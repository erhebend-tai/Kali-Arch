using CleanArch.Shared.Abstractions.DTOs.Payment;

namespace CleanArch.Shared.Abstractions.Application.Payment;

/// <summary>Payment processing - Stripe / PayPal / Adyen.</summary>
public interface IPaymentService
{
    Task<PaymentResult> ChargeAsync(
        PaymentRequest request, CancellationToken ct = default);

    Task<RefundResult> RefundAsync(
        string transactionId, decimal amount,
        CancellationToken ct = default);

    Task<PaymentStatus> GetStatusAsync(
        string transactionId, CancellationToken ct = default);
}
