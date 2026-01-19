using API.DataAccess.Repositories;
using API.DataAccess.Repositories.Abstract;
using API.Services;
using API.Services.Abstract;

namespace API;

public static class DependencyInjection
{
    public static IServiceCollection AddServiceLayer(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ITeacherRepository, TeacherRepository>();
        services.AddScoped<IJobCategoryRepository, JobCategoryRepository>();
        services.AddScoped<ITJCRepository, TJCRepository>();
        services.AddScoped<IPrincipalRepository, PrincipalRepository>();
        services.AddScoped<ICourseRepository, CourseRepository>();
        services.AddScoped<ITCRepository, TCRepository>();
        services.AddScoped<IGCRRepository, GCRRepository>();
        services.AddScoped<IGradeRepository, GradeRepository>();
        
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDevService, DevService>();
        services.AddScoped<IGCRService, GCRService>();

        return services;
    }
}