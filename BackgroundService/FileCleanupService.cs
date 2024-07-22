using Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackgroundService;

public class FileCleanupService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly IOptions<FileCleanupServiceOptions> _options;

    public FileCleanupService(IServiceProvider services,
        IOptions<FileCleanupServiceOptions> options)
    {
        _services = services;
        _options = options;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                if (string.IsNullOrEmpty(_options.Value.TempFolder) || !Directory.Exists(_options.Value.TempFolder))
                {
                    throw new Exception("[BG Service clean up error]: ошибка пути или существования иректории");
                }

                await using var scope = _services.CreateAsyncScope();
                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var directories = Directory.GetDirectories(_options.Value.TempFolder);

                foreach (var directory in directories)
                {
                    var files = Directory.GetFiles(directory);

                    if (files.Length == 0)
                    {
                        Directory.Delete(directory, true);
                        continue;
                    }

                    foreach (var filePath in files)
                    {
                        var fileInfo = new FileInfo(filePath);

                        var fileInDatabase = await db.Attachments
                            .FirstOrDefaultAsync(i => i.FilePath == filePath, cancellationToken: stoppingToken);

                        if (fileInDatabase == null &&
                            fileInfo.LastWriteTime.Add(_options.Value.CleanInterval) < DateTime.Now)
                        {
                            File.Delete(filePath);
                        }
                    }

                    if (Directory.GetFiles(directory).Length == 0)
                    {
                        Directory.Delete(directory, true);
                    }
                }
            }
            finally
            {
                await Task.Delay(_options.Value.CleanIntervalWorker, stoppingToken);
            }
        }
    }
}