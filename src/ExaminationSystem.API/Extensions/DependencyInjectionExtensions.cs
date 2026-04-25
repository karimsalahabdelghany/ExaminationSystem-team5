using ExaminationSystem.API.Background;
using ExaminationSystem.API.Services;
using ExaminationSystem.Application.Interfaces;

namespace ExaminationSystem.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddApplication();
        services.AddPersistence(configuration);
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();

        services.AddJwtAuthentication(configuration);
        services.AddAuthRateLimiting();

        services.Configure<AttemptAutoSubmitOptions>(configuration.GetSection(AttemptAutoSubmitOptions.SectionName));
        services.AddSingleton<AttemptAutoSubmitMetrics>();
        services.AddHostedService<AttemptDeadlineBackgroundService>();

        return services;
    }
}
