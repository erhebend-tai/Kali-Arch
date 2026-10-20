using MediatR;
using Microsoft.AspNetCore.Mvc;
using VirtualMachineService.Application.Interfaces.Webhooks;
using VirtualMachineService.Application.Notifications;

namespace VirtualMachineService.API.Controllers.Webhooks;

[ApiController]
[Route("api/webhooks/inbound")]
public sealed class InboundWebhookController(
    IWebhookVerifier verifier,
    IMediator        mediator,
    IConfiguration   config,
    ILogger<InboundWebhookController> logger) : ControllerBase
{
    // Receive webhooks FROM GitHub
    [HttpPost("github")]
    public async Task<IActionResult> ReceiveGitHubAsync(
        CancellationToken ct)
    {
        var signature = Request.Headers["X-Hub-Signature-256"]
            .FirstOrDefault();
        var secret = config["Webhooks:Inbound:GitHub:Secret"]!;

        var body = await ReadBodyAsync(ct);

        if (!verifier.VerifySignature(body, signature ?? "", secret))
        {
            logger.LogWarning("Invalid GitHub webhook signature");
            return Unauthorized();
        }

        var eventType = Request.Headers["X-GitHub-Event"]
            .FirstOrDefault();

        await mediator.Publish(
            new GitHubWebhookReceivedNotification(eventType!, body), ct);

        return Ok();
    }

    // Receive webhooks FROM Stripe
    [HttpPost("stripe")]
    public async Task<IActionResult> ReceiveStripeAsync(
        CancellationToken ct)
    {
        var signature = Request.Headers["Stripe-Signature"]
            .FirstOrDefault();
        var secret = config["Webhooks:Inbound:Stripe:Secret"]!;

        var body = await ReadBodyAsync(ct);

        if (!verifier.VerifySignature(body, signature ?? "", secret))
        {
            logger.LogWarning("Invalid Stripe webhook signature");
            return Unauthorized();
        }

        await mediator.Publish(
            new StripeWebhookReceivedNotification(body), ct);

        return Ok();
    }

    // Generic inbound — for custom integrations
    [HttpPost("{source}")]
    public async Task<IActionResult> ReceiveGenericAsync(
        string source, CancellationToken ct)
    {
        var secret = config[$"Webhooks:Inbound:{source}:Secret"];
        if (secret is null) return NotFound();

        var signature = Request.Headers["X-Webhook-Signature"]
            .FirstOrDefault();
        var body = await ReadBodyAsync(ct);

        if (!verifier.VerifySignature(body, signature ?? "", secret))
            return Unauthorized();

        await mediator.Publish(
            new GenericWebhookReceivedNotification(source, body), ct);

        return Ok();
    }

    private async Task<string> ReadBodyAsync(CancellationToken ct)
    {
        Request.EnableBuffering();
        using var reader = new StreamReader(
            Request.Body,
            leaveOpen: true);
        var body = await reader.ReadToEndAsync(ct);
        Request.Body.Position = 0;
        return body;
    }
}
