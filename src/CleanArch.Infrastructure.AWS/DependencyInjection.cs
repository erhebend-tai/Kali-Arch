using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CleanArch.Infrastructure.AWS;

/// <summary>Registers AWS implementations (S3, SQS/SNS, EC2, Cognito, SecretsManager).</summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAWSInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        _ = configuration;
        return services;
    }
}
