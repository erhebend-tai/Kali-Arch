using VirtualMachineService.API.Grpc.Interceptors;
using VirtualMachineService.API.Grpc.Services;

namespace VirtualMachineService.API.Grpc;

/// <summary>
/// gRPC wiring — the API project is a Library hosted elsewhere,
/// so registration lives in extensions the host calls:
///   builder.Services.AddVmGrpcServices(builder.Environment);
///   app.MapVmGrpcEndpoints();
/// </summary>
public static class GrpcServiceExtensions
{
    public static IServiceCollection AddVmGrpcServices(
        this IServiceCollection services,
        IHostEnvironment env)
    {
        services.AddGrpc(opts =>
        {
            opts.EnableDetailedErrors  = env.IsDevelopment();
            opts.MaxReceiveMessageSize = 16 * 1024 * 1024; // 16 MB
            opts.MaxSendMessageSize    = 16 * 1024 * 1024;

            // Order matters — first registered = outermost.
            opts.Interceptors.Add<LoggingInterceptor>();
            opts.Interceptors.Add<AuthInterceptor>();
            opts.Interceptors.Add<TenantInterceptor>();
            opts.Interceptors.Add<ValidationInterceptor>();
        })

        // gRPC → HTTP/JSON transcoding (REST bridge).
        .AddJsonTranscoding();

        // gRPC health checks.
        services.AddGrpcHealthChecks();

        // Server reflection — grpcurl / Postman discovery (dev only).
        if (env.IsDevelopment())
            services.AddGrpcReflection();

        return services;
    }

    public static WebApplication MapVmGrpcEndpoints(
        this WebApplication app)
    {
        app.UseGrpcWeb();

        app.MapGrpcService<VmGrpcService>()
            .EnableGrpcWeb()
            .RequireAuthorization();

        app.MapGrpcHealthChecksService();

        // gRPC reflection — for grpcurl / Postman.
        if (app.Environment.IsDevelopment())
            app.MapGrpcReflectionService();

        return app;
    }
}
