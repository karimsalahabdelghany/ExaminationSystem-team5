using ExaminationSystem.Application.Interfaces;
using ExaminationSystem.Application.Services.EmailService;
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
        ////services.AddScoped<AuditableEntityInterceptor>();
        services.AddDbContext<ApplicationContext>((serviceProvider, options) =>
        {
            var concurrencyInterceptor = serviceProvider.GetRequiredService<ConcurrencyInterceptor>();
            ////var auditableEntityInterceptor = serviceProvider.GetRequiredService<AuditableEntityInterceptor>();
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            //options.AddInterceptors(concurrencyInterceptor/*,auditableEntityInterceptor*/);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }).AddLogging(c => c.SetMinimumLevel(LogLevel.Information));

        services.AddIdentityCore<AppUser>(options =>
        {
            options.User.RequireUniqueEmail = true;
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;
            options.Password.RequiredLength = 8;
        })
            .AddRoles<IdentityRole<Guid>>()
            .AddEntityFrameworkStores<ApplicationContext>();
        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

        //services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IQuizAttemptAutoSubmitClaim, QuizAttemptAutoSubmitClaim>();
        services.AddScoped<ITokenServies, TokenServices>();
        services.AddScoped<ILoggerService, LoggerService>();

        services.Configure<EmailSettings>(
        configuration.GetSection("EmailSettings"));

        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
