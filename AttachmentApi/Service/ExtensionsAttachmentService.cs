using AttachmentApi.Service.Abstracts;
using Database.Database.Repository;
using Database.Database.Repository.Abstracts;

namespace AttachmentApi.Service;

public static class ExtensionsAttachmentService
{
    public static IServiceCollection AddAttachmentService(this IServiceCollection services,
        Action<AttachmentServiceOptions>? configure = null)
    {
        services.AddOptions<AttachmentServiceOptions>();

        if (configure != null)
            services.Configure(configure);

        services.AddScoped<IAttachmentRepository, AttachmentRepository>();
        services.AddScoped<IAttachmentService, AttachmentService>();
        return services;
    }
}