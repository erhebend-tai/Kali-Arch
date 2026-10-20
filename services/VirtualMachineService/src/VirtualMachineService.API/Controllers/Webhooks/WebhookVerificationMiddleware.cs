using VirtualMachineService.Application.Interfaces.Webhooks;

namespace VirtualMachineService.API.Controllers.Webhooks;

/// <summary>
/// Rejects unsigned/mis-signed inbound webhooks before routing —
/// verifies X-Webhook-Signature against the per-source secret
/// (Webhooks:Inbound:{source}:Secret). Buffers the body so the
/// controller can re-read it.
/// </summary>
public sealed class WebhookVerificationMiddleware(
    RequestDelegate next,
    IWebhookVerifier verifier,
    IConfiguration config,
    ILogger<WebhookVerificationMiddleware> logger)
{
    private static readonly PathString InboundPath =
        new("/api/webhooks/inbound");

    public async Task InvokeAsync(HttpContext context)
    {
        if (!context.Request.Path.StartsWithSegments(
                InboundPath, out var remainder)
            || context.Request.Method != HttpMethods.Post)
        {
            await next(context);
            return;
        }

        // /api/webhooks/inbound/{source}
        var source = remainder.Value?.Trim('/').Split('/')
            .FirstOrDefault();

        var secret = source is null
            ? null
            : config[$"Webhooks:Inbound:{source}:Secret"];

        var signature = context.Request.Headers["X-Webhook-Signature"]
            .FirstOrDefault();

        context.Request.EnableBuffering();
        using var reader = new StreamReader(
            context.Request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync();
        context.Request.Body.Position = 0;

        if (secret is null
            || !verifier.VerifySignature(body, signature ?? "", secret))
        {
            logger.LogWarning(
                "Rejected webhook from {Source} — bad signature",
                source ?? "unknown");
            context.Response.StatusCode =
                StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}
