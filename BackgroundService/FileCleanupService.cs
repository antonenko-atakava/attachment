using Database.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace BackgroundService;

public class FileCleanupService : Microsoft.Extensions.Hosting.BackgroundService
{
    private readonly IServiceProvider _services;
    private readonly ILogger<FileCleanupService> _logger;
    private readonly IOptions<FileCleanupServiceOptions> _options;

    public FileCleanupService(IServiceProvider services, ILogger<FileCleanupService> logger,
        IOptions<FileCleanupServiceOptions> options)
    {
        _logger = logger;
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
                    _logger.LogError("Указанный путь пуст или папки не существует");
                    throw new Exception();
                }

                await using var scope = _services.CreateAsyncScope();

                var db = scope.ServiceProvider.GetRequiredService<DatabaseContext>();

                var fileOnDisk = Directory.GetFiles(_options.Value.TempFolder);

                foreach (var filePath in fileOnDisk)
                {
                    var fileName = Path.GetFileName(filePath);

                    var fileInfo = new FileInfo(filePath);

                    var fileInDatabase = await db.Attachments
                        .FirstOrDefaultAsync(i => i.Extension == fileName, cancellationToken: stoppingToken);

                    if (fileInDatabase == null && fileInfo.LastWriteTime.Add(_options.Value.CleanInterval) <
                        DateTime.Now)
                    {
                        _logger.LogInformation($"файл {fileInfo.Name} должен удалится");
                        File.Delete(filePath);
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