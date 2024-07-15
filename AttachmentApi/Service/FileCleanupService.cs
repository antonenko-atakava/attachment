using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AttachmentApi.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AttachmentApi.Service;

public class FileCleanupService : BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IConfiguration _configuration;

    public FileCleanupService(IServiceProvider services, IConfiguration configuration)
    {
        _services = services;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanupFilesAsync();
            await Task.Delay(int.Parse(_configuration["CleanInterval"] ?? throw new NullReferenceException()),
                stoppingToken);
        }
    }

    private async Task CleanupFilesAsync()
    {
        using (var scope = _services.CreateScope())
        {
            var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

            var fileOnDisk =
                Directory.GetFiles(_configuration["FolderFiles:Temp"] ?? throw new NullReferenceException());

            foreach (var filePath in fileOnDisk)
            {
                var fileName = Path.GetFileName(filePath);

                var fileInfo = new FileInfo(filePath);

                var fileInDatabase = await db.Attachments
                    .FirstOrDefaultAsync(i => i.Extension == fileName);

                if (fileInDatabase == null && fileInfo.LastWriteTime.AddSeconds(20) < DateTime.Now)
                {
                    Console.WriteLine($"файл {fileInfo.Name} должен удалится");
                    File.Delete(filePath);
                }
            }
        }
    }
}