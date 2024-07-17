namespace BackgroundService;

public static class Extensions
{
    public static IServiceCollection AddFileCleanupService(this IServiceCollection services,
        Action<FileCleanupServiceOptions>? configure = null)
    {
        services.AddOptions<FileCleanupServiceOptions>();

        if (configure != null)
            services.Configure(configure);
        
        services.AddHostedService<FileCleanupService>();
        return services;
    }
}