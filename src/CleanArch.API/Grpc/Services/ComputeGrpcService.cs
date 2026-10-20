using CleanArch.Application.Commands.Compute;
using CleanArch.Application.Queries.Compute;
using Grpc.Core;
using MediatR;

namespace CleanArch.API.Grpc.Services;

/// <summary>
/// gRPC = another entry point over the same application
/// pipeline — identical to the REST controller's route.
/// </summary>
public sealed class ComputeGrpcService(ISender sender)
    : ComputeService.ComputeServiceBase
{
    public override async Task<ProvisionVmReply> ProvisionVm(
        ProvisionVmRequest request, ServerCallContext context)
    {
        var result = await sender.Send(
            new ProvisionVmCommand(
                request.Name, request.ResourceGroup,
                request.Region, request.OsType, request.Size),
            context.CancellationToken);

        return new ProvisionVmReply
        {
            Id = result.VmId.ToString(),
            Name = result.Name,
            Status = result.Status
        };
    }

    public override async Task<GetVmReply> GetVm(
        GetVmRequest request, ServerCallContext context)
    {
        var vm = await sender.Send(
            new GetVmByIdQuery(Guid.Parse(request.Id)),
            context.CancellationToken);

        if (vm is null)
            throw new RpcException(
                new Status(StatusCode.NotFound,
                    $"VM {request.Id} not found."));

        return new GetVmReply
        {
            Id = vm.Id.ToString(),
            Name = vm.Name,
            Region = vm.Region,
            Size = vm.Size,
            OsType = vm.OsType,
            Status = vm.Status,
            ResourceGroup = vm.ResourceGroup
        };
    }

    public override async Task<ListVmsReply> ListVms(
        ListVmsRequest request, ServerCallContext context)
    {
        var region = string.IsNullOrEmpty(request.Region)
            ? null : request.Region;

        var vms = await sender.Send(
            new GetAllVmsQuery(region, request.RunningOnly),
            context.CancellationToken);

        var reply = new ListVmsReply();
        reply.Vms.AddRange(vms.Select(v => new VmSummary
        {
            Id = v.Id.ToString(),
            Name = v.Name,
            Region = v.Region,
            Size = v.Size,
            Status = v.Status
        }));
        return reply;
    }

    public override async Task<StartVmReply> StartVm(
        StartVmRequest request, ServerCallContext context)
    {
        await sender.Send(
            new StartVmCommand(Guid.Parse(request.Id)),
            context.CancellationToken);

        return new StartVmReply { Success = true };
    }
}
