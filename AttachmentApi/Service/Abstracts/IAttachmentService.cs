using AttachmentApi.Database.DTO;

namespace AttachmentApi.Service.Abstracts;

public interface IAttachmentService
{
    Task<AttachmentDto> GetById(int id);
    Task<AttachmentDto> GetByName(string name);
    Task<List<AttachmentDto>> Select();
    Task<AttachmentDto> Upload(IFormFile file);
    Task<AttachmentDto> Create(AttachmentDto entity);
    Task<bool> Delete(int id);
}