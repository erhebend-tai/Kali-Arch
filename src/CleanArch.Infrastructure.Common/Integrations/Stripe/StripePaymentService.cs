using CleanArch.Application.Interfaces.Payment;
using Stripe;

namespace CleanArch.Infrastructure.Common.Integrations.Stripe;

/// <summary>Stripe implementation of IPaymentService.</summary>
public sealed class StripePaymentService(
    StripeClient client) : IPaymentService
{
    public async Task<PaymentResult> ChargeAsync(
        PaymentRequest request, CancellationToken ct = default)
    {
        var service = new PaymentIntentService(client);

        var intent = await service.CreateAsync(
            new PaymentIntentCreateOptions
            {
                Amount = (long)(request.Amount * 100),
                Currency = request.Currency,
                PaymentMethod = request.PaymentMethodId,
                Confirm = true,
                Metadata = new Dictionary<string, string>
                {
                    ["order_id"] = request.OrderId,
                    ["customer_id"] = request.CustomerId
                }
            }, cancellationToken: ct);

        var status = intent.Status switch
        {
            "succeeded" => PaymentStatus.Succeeded,
            "processing" => PaymentStatus.Processing,
            "requires_action" => PaymentStatus.RequiresAction,
            "canceled" => PaymentStatus.Cancelled,
            _ => PaymentStatus.Failed
        };

        return new PaymentResult(
            intent.Id, status,
            intent.Amount / 100m, intent.Currency);
    }
}
