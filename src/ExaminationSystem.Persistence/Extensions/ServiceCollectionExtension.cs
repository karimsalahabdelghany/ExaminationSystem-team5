using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Persistence.Interceptors;
using ExaminationSystem.Persistence.Repositories;
using ExaminationSystem.Persistence.Services;
using Microsoft.Extensions.Logging;

namespace ExaminationSystem.Persistence.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<ConcurrencyInterceptor>();
        services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
        {
            var concurrencyInterceptor = serviceProvider.GetRequiredService<ConcurrencyInterceptor>();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            options.AddInterceptors(concurrencyInterceptor);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }).AddLogging(c => c.SetMinimumLevel(LogLevel.Information));

        services.AddIdentityCore<User>(options =>
            {
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = true;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 8;
            })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationContext>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IDbSession, DbSession>();

        return services;
    }
}