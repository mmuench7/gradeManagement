using API.DataAccess;
using API.Services.Abstract;

namespace API.Services;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration config)
    {
        services.AddDataAccessLayer(config);

        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IGCRService, GCRService>();

        return services;
    }
}
