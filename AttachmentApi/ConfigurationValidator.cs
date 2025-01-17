using AttachmentApi.Service;
using BackgroundService;

namespace AttachmentApi;

public static class ConfigurationValidator
{
    public static void ValidateOptions(FileCleanupServiceOptions cleanupOptions,
        AttachmentServiceOptions attachmentOptions)
    {
        if (
            string.IsNullOrEmpty(cleanupOptions.TempFolder) ||
            string.IsNullOrEmpty(attachmentOptions.TempFolder) ||
            string.IsNullOrEmpty(attachmentOptions.FileManagerFolder)
        )
        {
            throw new ApplicationException("В конфиге пустые поля.");
        }
    }
}