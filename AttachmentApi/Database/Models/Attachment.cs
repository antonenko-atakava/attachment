using System;

namespace AttachmentApi.Database.Models;

public class Attachment
{
    public int Id { get; set; }
    public string Extension { get; set; }
    public long FileSize { get; set; }
    public string FilePath { get; set; }
    public DateTime CreateAt { get; set; }
    public bool Success { get; set; }
}

