using System.Collections.Generic;
using System.Threading.Tasks;
using AttachmentApi.Database.DTO;
using Microsoft.AspNetCore.Http;

namespace AttachmentApi.Service.Abstracts;

public interface IAttachmentService
{
    Task<AttachmentDto> GetById(int id);
    Task<ICollection<AttachmentDto>> Pagination(uint size, uint page);
    Task<string> Upload(IFormFile file);
    Task<AttachmentDto> Create(string filePath);
    Task<bool> Delete(int id);
}