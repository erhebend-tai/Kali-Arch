using CleanArch.Application.Commands.Compute;
using CleanArch.Application.DTOs.Compute;
using CleanArch.Domain.Entities.Compute;
using Mapster;

namespace CleanArch.Application.Mappings;

/// <summary>
/// Mapster profile — DTO↔entity translation lives in
/// Application, entities never cross the boundary.
/// </summary>
public sealed class ComputeMappingProfile : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<VirtualMachine, VmSummaryDto>()
            .Map(dest => dest.Name, src => src.Name.Value)
            .Map(dest => dest.Region, src => src.Region.Value)
            .Map(dest => dest.Size, src => src.Size.Value)
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.NewConfig<VirtualMachine, VmDetailsDto>()
            .Map(dest => dest.Name, src => src.Name.Value)
            .Map(dest => dest.Region, src => src.Region.Value)
            .Map(dest => dest.Size, src => src.Size.Value)
            .Map(dest => dest.OsType, src => src.OsType.ToString())
            .Map(dest => dest.Status, src => src.Status.ToString());

        config.NewConfig<ProvisionVmResult, ProvisionVmResponse>()
            .Map(dest => dest.Id, src => src.VmId);
    }
}
