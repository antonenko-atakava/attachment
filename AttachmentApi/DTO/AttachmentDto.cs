namespace AttachmentApi.DTO;

public class AttachmentDto
{
    public string Id { get; set; }
    public string Extension { get; set; }
    public long FileSize { get; set; }
    public string FilePath { get; set; }    
    public DateTime CreateAt { get; set; }
    public bool Success { get; set; }
}