using AttachmentApi.DTO;
using Microsoft.EntityFrameworkCore.Storage;

namespace AttachmentApi.Service.Abstracts;

public interface IAttachmentService
{
    Task<AttachmentDto> GetById(string id);
    Task<ICollection<AttachmentDto>> Pagination(uint size, uint page);
    Task<string> Upload(IFormFile file);
    Task<AttachmentDto> Create(string filePath, IDbContextTransaction transaction);
    Task<bool> Delete(string id);
}