using API.DataAccess.Repositories;
using API.DataAccess.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;

namespace API.DataAccess;

public static class DependencyInjection
{
    public static IServiceCollection AddDataAccessLayer(this IServiceCollection services, IConfiguration config)
    {
        string? connectionString = config.GetConnectionString("DefaultConnection");

        services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        });

        services.AddScoped<IAuthRepository, AuthRepository>();
        services.AddScoped<IGCRRepository, GCRRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IPrincipalRepository, PrincipalRepository>();
        services.AddScoped<IGCRRepository, GCRRepository>();

        return services;
    }
}
