namespace ExaminationSystem.API.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddApiDependencies(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddControllers();
        services.AddOpenApi();
        services.AddApplication();
        services.AddPersistence(configuration);

        return services;
    }
}
