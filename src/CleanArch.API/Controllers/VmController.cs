using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Queries.Compute;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CleanArch.API.Controllers;

/// <summary>
/// Thin dispatching boundary — no business logic.
/// Controller → ISender → MediatR pipeline → handler.
/// </summary>
[ApiController]
[Route("api/compute/vms")]
public class VmController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Provision(
        [FromBody] ProvisionVmRequest request,
        CancellationToken ct)
    {
        var command = new ProvisionVmCommand(
            request.Name,
            request.ResourceGroup,
            request.Region,
            request.OsType,
            request.Size);

        var result = await sender.Send(command, ct);
        return CreatedAtAction(
            nameof(GetById), new { id = result.VmId }, result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetVmByIdQuery(id), ct);
        return result is null ? NotFound() : Ok(result);
    }

    [HttpGet]
    public async Task<IActionResult> List(
        [FromQuery] string? region,
        [FromQuery] bool runningOnly,
        CancellationToken ct) =>
        Ok(await sender.Send(
            new GetAllVmsQuery(region, runningOnly), ct));

    [HttpPost("{id:guid}/start")]
    public async Task<IActionResult> Start(Guid id, CancellationToken ct)
    {
        await sender.Send(new StartVmCommand(id), ct);
        return Accepted();
    }

    [HttpPost("{id:guid}/stop")]
    public async Task<IActionResult> Stop(Guid id, CancellationToken ct)
    {
        await sender.Send(new StopVmCommand(id), ct);
        return Accepted();
    }
}

public sealed record ProvisionVmRequest(
    string Name,
    string ResourceGroup,
    string Region,
    string OsType,
    string Size);
