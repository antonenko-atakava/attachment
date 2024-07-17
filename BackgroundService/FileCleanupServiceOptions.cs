namespace BackgroundService;

public class FileCleanupServiceOptions
{
    public TimeSpan CleanInterval { get; set; } 
    public TimeSpan CleanIntervalWorker { get; set; }
    
    public string TempFolder { get; set; }
}