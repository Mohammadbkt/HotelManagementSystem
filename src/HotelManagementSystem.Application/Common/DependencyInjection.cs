using System.Reflection;
using FluentValidation;
using HotelManagementSystem.Application.Common.Behaviors;
using Microsoft.Extensions.DependencyInjection;

namespace HotelManagementSystem.Application.Common
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
                cfg.AddOpenBehavior(typeof(LoggingBehavior<,>));
                cfg.AddOpenBehavior(typeof(UnhandledExceptionBehavior<,>));
                cfg.AddOpenBehavior(typeof(ValidationBehaviors<,>));
            });

            return services;

        }
    }
}