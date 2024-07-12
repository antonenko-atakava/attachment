using AttachmentApi.Database.DTO;

namespace AttachmentApi.Service.Abstracts;

public interface IAttachmentService
{
    Task<AttachmentDto> GetById(int id);
    Task<ICollection<AttachmentDto>> Pagination(uint size, uint page);
    Task<string> Upload(IFormFile file);
    Task<AttachmentDto> Create(string filePath);
    Task<bool> Delete(int id);
}