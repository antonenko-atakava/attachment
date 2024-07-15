using System;

namespace AttachmentApi.Database.DTO;

public class AttachmentDto
{
    public int Id { get; set; }
    public string Extension { get; set; }
    public long FileSize { get; set; }
    public string FilePath { get; set; }
    public DateTime CreateAt { get; set; }
    public bool Success { get; set; }
}