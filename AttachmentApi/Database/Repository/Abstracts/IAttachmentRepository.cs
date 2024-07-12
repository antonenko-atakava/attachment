using AttachmentApi.Database.Models;

namespace AttachmentApi.Database.Repository.Abstracts;

public interface IAttachmentRepository
{
    Task<Attachment?> GetById(int id);
    Task<Attachment?> GetByName(string name);
    Task<List<Attachment>> Select();
    Task<Attachment> Create(Attachment entity);
    Task<bool> Delete(Attachment entity);
    Task SaveAsync();
}