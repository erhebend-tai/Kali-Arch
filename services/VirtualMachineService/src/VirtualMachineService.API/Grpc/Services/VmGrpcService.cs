using CleanArch.Shared.Results;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using MediatR;
using VirtualMachineService.Application.Commands.Compute;
using VirtualMachineService.Application.Common;
using VirtualMachineService.Application.Queries.Compute;
using VirtualMachineService.Application.Reactive.Streams;

namespace VirtualMachineService.API.Grpc.Services;

/// <summary>
/// gRPC = another entry point over the same application
/// pipeline — identical to the REST controller's route.
/// Streaming RPCs live in VmStreamingService (partial).
/// </summary>
public sealed partial class VmGrpcService(
    ISender        sender,
    VmStatusStream statusStream,
    ILogger<VmGrpcService> logger)
    : VmService.VmServiceBase
{
    // ── Unary ─────────────────────────────────────────────────
    public override async Task<ProvisionVmResponse> ProvisionVm(
        ProvisionVmRequest request,
        ServerCallContext  context)
    {
        var result = await sender.Send(
            new ProvisionVmCommand(
                request.Name,
                request.Region,
                request.Size,
                request.OsType,
                request.ResourceGroup,
                request.RequestedBy,
                request.TenantId),
            context.CancellationToken);

        if (result.IsFailure)
            throw new RpcException(new Status(
                MapErrorToStatusCode(result.Error.Type),
                result.Error.Description));

        return new ProvisionVmResponse
        {
            VmId   = result.Value.VmId.ToString(),
            Name   = result.Value.Name,
            Status = result.Value.Status
        };
    }

    public override async Task<VmDetails> GetVm(
        GetVmRequest      request,
        ServerCallContext context)
    {
        var result = await sender.Send(
            new GetVmByIdQuery(
                Guid.Parse(request.VmId), request.TenantId),
            context.CancellationToken);

        if (result.IsFailure)
            throw new RpcException(new Status(
                MapErrorToStatusCode(result.Error.Type),
                result.Error.Description));

        var vm = result.Value;
        return new VmDetails
        {
            VmId          = vm.Id.ToString(),
            Name          = vm.Name,
            Region        = vm.Region,
            Size          = vm.Size,
            OsType        = vm.OsType,
            Status        = vm.Status,
            ResourceGroup = vm.ResourceGroup,
            Provider      = vm.Provider,
            CreatedBy     = vm.CreatedBy,
            TenantId      = vm.TenantId,
            CreatedAt     = ToTimestamp(vm.CreatedAt),
            UpdatedAt     = vm.UpdatedAt is { } updated
                ? ToTimestamp(updated)
                : null
        };
    }

    public override async Task<ListVmsResponse> ListVms(
        ListVmsRequest    request,
        ServerCallContext context)
    {
        var page     = request.Page?.Page     ?? 1;
        var pageSize = request.Page?.PageSize ?? 20;

        var result = await sender.Send(
            new GetAllVmsQuery(
                TenantId: request.TenantId,
                Status:   NullIfEmpty(request.Status),
                Region:   NullIfEmpty(request.Region),
                Search:   NullIfEmpty(request.Search),
                Page:     page,
                PageSize: pageSize),
            context.CancellationToken);

        var response = new ListVmsResponse();
        response.Vms.AddRange(result.Items.Select(vm => new VmSummary
        {
            VmId          = vm.Id.ToString(),
            Name          = vm.Name,
            Region        = vm.Region,
            Size          = vm.Size,
            OsType        = vm.OsType,
            Status        = vm.Status,
            ResourceGroup = vm.ResourceGroup,
            CreatedAt     = ToTimestamp(vm.CreatedAt)
        }));

        response.PageInfo = new CleanArch.Shared.Grpc.Common.PageInfo
        {
            TotalCount  = (int)result.TotalCount,
            Page        = result.Page,
            PageSize    = result.PageSize,
            TotalPages  = (int)Math.Ceiling(
                (double)result.TotalCount / result.PageSize),
            HasNext     = result.Page * result.PageSize < result.TotalCount,
            HasPrevious = result.Page > 1
        };

        return response;
    }

    public override Task<VmActionResponse> StartVm(
        VmActionRequest   request,
        ServerCallContext context) =>
        RunActionAsync(
            new StartVmCommand(
                Guid.Parse(request.VmId), request.TenantId),
            context);

    public override Task<VmActionResponse> StopVm(
        VmActionRequest   request,
        ServerCallContext context) =>
        RunActionAsync(
            new StopVmCommand(
                Guid.Parse(request.VmId), request.TenantId),
            context);

    public override Task<VmActionResponse> DeallocateVm(
        VmActionRequest   request,
        ServerCallContext context) =>
        RunActionAsync(
            new DeallocateVmCommand(
                Guid.Parse(request.VmId), request.TenantId),
            context);

    public override Task<VmActionResponse> ResizeVm(
        ResizeVmRequest   request,
        ServerCallContext context) =>
        RunActionAsync(
            new ResizeVmCommand(
                Guid.Parse(request.VmId),
                request.NewSize, request.TenantId),
            context);

    public override async Task<Empty> DeleteVm(
        VmActionRequest   request,
        ServerCallContext context)
    {
        var deletedBy = context.UserState["CurrentUser"]
            is CleanArch.Shared.Grpc.Abstractions.UserInfo user
            ? user.Email
            : "unknown";

        var result = await sender.Send(
            new DeleteVmCommand(
                Guid.Parse(request.VmId), request.TenantId, deletedBy),
            context.CancellationToken);

        if (result.IsFailure)
            throw new RpcException(new Status(
                MapErrorToStatusCode(result.Error.Type),
                result.Error.Description));

        return new Empty();
    }

    // ── Client Streaming — Bulk Provision ─────────────────────
    public override async Task<BulkProvisionResponse> BulkProvision(
        IAsyncStreamReader<ProvisionVmRequest> requestStream,
        ServerCallContext                      context)
    {
        var results = new List<ProvisionVmResponse>();
        var failed  = 0;

        await foreach (var request in requestStream.ReadAllAsync(
            context.CancellationToken))
        {
            var result = await sender.Send(
                new ProvisionVmCommand(
                    request.Name, request.Region, request.Size,
                    request.OsType, request.ResourceGroup,
                    request.RequestedBy, request.TenantId),
                context.CancellationToken);

            if (result.IsSuccess)
            {
                results.Add(new ProvisionVmResponse
                {
                    VmId   = result.Value.VmId.ToString(),
                    Name   = result.Value.Name,
                    Status = result.Value.Status
                });
            }
            else
            {
                failed++;
                logger.LogWarning(
                    "Bulk provision failed for {Name}: {Error}",
                    request.Name, result.Error.Description);
            }
        }

        var response = new BulkProvisionResponse
        {
            Total     = results.Count + failed,
            Succeeded = results.Count,
            Failed    = failed
        };
        response.Results.AddRange(results);
        return response;
    }

    // ── Helpers ───────────────────────────────────────────────
    private async Task<VmActionResponse> RunActionAsync(
        ICommand<Result<VmActionResult>> command,
        ServerCallContext context)
    {
        var result = await sender.Send(
            command, context.CancellationToken);

        if (result.IsFailure)
            throw new RpcException(new Status(
                MapErrorToStatusCode(result.Error.Type),
                result.Error.Description));

        return new VmActionResponse
        {
            Success = true,
            Message = result.Value.Message,
            VmId    = result.Value.VmId.ToString(),
            Status  = result.Value.Status
        };
    }

    private static string? NullIfEmpty(string value) =>
        string.IsNullOrEmpty(value) ? null : value;

    private static Timestamp ToTimestamp(DateTime value) =>
        Timestamp.FromDateTime(
            DateTime.SpecifyKind(value, DateTimeKind.Utc));

    internal static StatusCode MapErrorToStatusCode(
        ErrorType errorType) =>
        errorType switch
        {
            ErrorType.NotFound     => StatusCode.NotFound,
            ErrorType.Validation   => StatusCode.InvalidArgument,
            ErrorType.Conflict     => StatusCode.AlreadyExists,
            ErrorType.Unauthorized => StatusCode.Unauthenticated,
            ErrorType.Forbidden    => StatusCode.PermissionDenied,
            ErrorType.Domain       => StatusCode.FailedPrecondition,
            _                      => StatusCode.Internal
        };
}
