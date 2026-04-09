using ExaminationSystem.Application.Common.Behaviours;
using ExaminationSystem.Application.Common.Helper;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ExaminationSystem.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ServiceCollectionExtension).Assembly));
        services.AddValidatorsFromAssembly(typeof(ServiceCollectionExtension).Assembly);
        services.AddScoped<TransactionScope>();
        services.AddTransient(typeof(IPipelineBehavior<,>),
                             typeof(UnitOfWorkBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>),
                            typeof(ValidationBehaviour<,>));
        return services;
    }
}
