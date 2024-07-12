using AttachmentApi.Database;
using Microsoft.EntityFrameworkCore;

namespace AttachmentApi.Infrastructure;

public class FileCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(10);

    public FileCleanupService(IServiceProvider services)
    {
        _services = services;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupFilesAsync();
            await Task.Delay(_cleanupInterval, stoppingToken);
        }
    }

    private async Task CleanupFilesAsync()
    {
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var fileOnDisk = Directory.GetFiles(@"C:\FilesManager");

            foreach (var filePath in fileOnDisk)
            {
                var fileName = Path.GetFileName(filePath);

                var fileInDatabase = await db.Attachments.FirstOrDefaultAsync(i => i.Extension == fileName);

                if (fileInDatabase == null)
                    File.Delete(filePath);
            }
        }
    }
}