using HotelManagementSystem.Application.Common;
using HotelManagementSystem.Infrastructure;

namespace HotelManagementSystem.Api;

public static class DependencyInjection
{
    public static IServiceCollection AddApi(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOpenApi();

        services.AddApplication();
        services.AddInfrastructure(configuration);

        services.AddProblemDetails();
        

        services.AddControllers();
        services.AddOpenApi();

        return services;
    }
}