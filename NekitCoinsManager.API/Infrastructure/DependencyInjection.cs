using MapsterMapper;
using NekitCoinsManager.API.Middleware;

namespace NekitCoinsManager.API.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {
        MappingConfig.ConfigureMapping();

        services.AddTransient<IMapper, Mapper>();
        
        return services;
    }
    
    public static IApplicationBuilder AddMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ErrorHandlingMiddleware>();
        return app;
    }
} 