using CleanArch.Application.DTOs.Compute;
using CleanArch.Domain.Entities.Compute;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Application.Mappings;

public static class MappingExtensions
{
    public static VmSummaryDto ToSummaryDto(this VirtualMachine vm) =>
        vm.Adapt<VmSummaryDto>();

    public static VmDetailsDto ToDetailsDto(this VirtualMachine vm) =>
        vm.Adapt<VmDetailsDto>();

    /// <summary>
    /// Registers the TypeAdapterConfig — scans this assembly for
    /// all IRegister implementations.
    /// </summary>
    public static IServiceCollection AddMappings(
        this IServiceCollection services)
    {
        var config = TypeAdapterConfig.GlobalSettings;
        config.Scan(typeof(ComputeMappingProfile).Assembly);
        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}
